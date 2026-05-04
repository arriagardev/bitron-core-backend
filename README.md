# Bitron Core Backend

Backend central para el sistema de análisis de calidad por visión artificial en líneas de ensamble de módulos de control eléctrico para asientos vehiculares. Consume veredictos de inspección desde dispositivos Edge (Raspberry Pi) vía MQTT, los persiste en PostgreSQL y los emite en tiempo real al dashboard web mediante SignalR.

## Arquitectura

```
Raspberry Pi (Edge)
    │  MQTT  bitron/{linea}/inspeccion/resultado
    ▼
Eclipse Mosquitto (Broker)
    │
    ▼
BitronCore.Api  ──────────────────────────────────────────────┐
    │                                                          │
    ├── MqttBackgroundService (Infrastructure)                 │
    │       │ deserializa + valida                             │
    │       ▼                                                  │
    │   InspeccionService (Application)                        │
    │       │ idempotencia por transaccion_id                  │
    │       ├──► PostgreSQL (vía EF Core)                      │
    │       └──► SignalRInspeccionNotifier ──► InspeccionHub   │
    │                                              │           │
    └── REST API ◄─────────────────── Dashboard ◄─┘           │
              GET /api/inspecciones                            │
              GET /api/estadisticas               WebSocket ◄─┘
              GET /api/dispositivos
```

### Estructura del proyecto — Clean Architecture

```
BitronCore.sln
├── src/
│   ├── BitronCore.Domain/          # Entidades y enums (sin dependencias externas)
│   ├── BitronCore.Application/     # DTOs, interfaces, InspeccionService
│   ├── BitronCore.Infrastructure/  # EF Core, PostgreSQL, cliente MQTT
│   └── BitronCore.Api/             # Controllers REST, SignalR Hub, Program.cs
└── tests/
    └── BitronCore.Tests/           # Unit tests (xUnit + Moq + FluentAssertions)
```

## Stack tecnológico

| Componente | Tecnología |
|---|---|
| Runtime | .NET 8 |
| Base de datos | PostgreSQL 16 |
| ORM | Entity Framework Core 8 + Npgsql |
| Message Broker | Eclipse Mosquitto 2 |
| Cliente MQTT | MQTTnet 4 (ManagedMqttClient con reconexión automática) |
| Tiempo real | ASP.NET Core SignalR |
| Documentación API | Swagger / OpenAPI |
| Contenedores | Docker + Docker Compose |

## Requisitos

- [Docker](https://docs.docker.com/get-docker/) y Docker Compose

## Inicio rápido

```bash
# Clonar el repositorio
git clone <url-del-repositorio>
cd bitron-core-backend

# Levantar todos los servicios (PostgreSQL + Mosquitto + API)
docker-compose up -d

# Verificar que la API está corriendo
curl http://localhost:5000/api/estadisticas
```

La API aplica las migraciones de base de datos automáticamente al iniciar.

Swagger UI disponible en: `http://localhost:5000/swagger`

## Configuración

Todas las opciones se configuran en `src/BitronCore.Api/appsettings.json` o mediante variables de entorno en Docker Compose.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bitroncore;Username=bitron;Password=bitron123"
  },
  "Mqtt": {
    "Host": "localhost",
    "Port": 1883,
    "ClientId": "bitron-core-backend",
    "TopicPattern": "bitron/+/inspeccion/resultado",
    "KeepAliveSeconds": 60,
    "ReconnectDelaySeconds": 5
  }
}
```

El patrón de tópico `bitron/+/inspeccion/resultado` soporta múltiples líneas de ensamble; el segmento `+` se extrae automáticamente como el nombre de la línea (`linea1`, `linea2`, etc.).

## API REST

### `GET /api/inspecciones`

Lista paginada de inspecciones con filtros opcionales.

| Query param | Tipo | Ejemplo |
|---|---|---|
| `page` | int | `1` |
| `pageSize` | int (máx 100) | `20` |
| `veredicto` | string | `OK` \| `NG` |
| `linea` | string | `linea1` |
| `dispositivoId` | string | `rasp_ensamble_L1` |
| `fechaDesde` | datetime UTC | `2026-04-27T00:00:00Z` |
| `fechaHasta` | datetime UTC | `2026-04-27T23:59:59Z` |

**Respuesta:**
```json
{
  "items": [ { "transaccionId": "...", "veredictoGlobal": "NG", "analisisRois": [...] } ],
  "totalCount": 1540,
  "page": 1,
  "pageSize": 20,
  "totalPages": 77
}
```

### `GET /api/inspecciones/{transaccionId}`

Detalle completo de una inspección incluyendo todos los ROIs analizados.

### `GET /api/estadisticas`

Métricas agregadas. Acepta los mismos filtros de `linea`, `dispositivoId`, `fechaDesde` y `fechaHasta`.

```json
{
  "total": 1540,
  "okCount": 1432,
  "ngCount": 108,
  "yieldRate": 93.0,
  "promTiempoMs": 412.5
}
```

### `GET /api/dispositivos`

Dispositivos Edge registrados con su última actividad y total de inspecciones.

## SignalR — Tiempo real

**Endpoint:** `ws://host:5000/hubs/inspeccion`

El frontend se suscribe a grupos para recibir actualizaciones:

| Método (cliente → servidor) | Descripción |
|---|---|
| `JoinLinea("linea1")` | Recibir eventos solo de esa línea |
| `LeaveLinea("linea1")` | Dejar de escuchar esa línea |
| `JoinTodas()` | Recibir eventos de todas las líneas |

**Evento emitido por el servidor:**

```
NuevaInspeccion  →  InspeccionResponseDto
```

## Modelo de datos MQTT

El dispositivo Edge publica en `bitron/{linea}/inspeccion/resultado`:

```json
{
  "transaccion_id": "550e8400-e29b-41d4-a716-446655440000",
  "dispositivo_id": "rasp_ensamble_L1",
  "timestamp": "2026-04-27T19:35:00Z",
  "metadatos": {
    "modelo_detectado": "A",
    "tiempo_total_ms": 435,
    "version_modelo_knn": "v1.2.0"
  },
  "veredicto_global": "NG",
  "evidencia_url": "http://10.0.0.5/auditoria/ng_20260427_193500.jpg",
  "analisis_roi": [
    { "zona": "Asiento", "densidad": 85.2, "brillo_promedio": 180.5, "es_ok": true },
    { "zona": "SET",     "densidad": 88.1, "brillo_promedio": 190.0, "es_ok": true },
    { "zona": "Boton 1", "densidad": 12.5, "brillo_promedio": 45.2,  "es_ok": false }
  ]
}
```

> La imagen **no** se envía en Base64 por MQTT para no saturar el broker. Se envía una URL que apunta al servidor Edge local; el campo `evidencia_url` solo está presente cuando `veredicto_global` es `NG`.

## Pruebas

### Simular un mensaje desde el Edge

Con el broker Mosquitto corriendo:

```bash
mosquitto_pub -h localhost -t "bitron/linea1/inspeccion/resultado" -m '{
  "transaccion_id":"550e8400-e29b-41d4-a716-446655440000",
  "dispositivo_id":"rasp_ensamble_L1",
  "timestamp":"2026-04-27T19:35:00Z",
  "metadatos":{"modelo_detectado":"A","tiempo_total_ms":435,"version_modelo_knn":"v1.2.0"},
  "veredicto_global":"NG",
  "evidencia_url":"http://10.0.0.5/auditoria/ng_20260427_193500.jpg",
  "analisis_roi":[
    {"zona":"Asiento","densidad":85.2,"brillo_promedio":180.5,"es_ok":true},
    {"zona":"SET","densidad":88.1,"brillo_promedio":190.0,"es_ok":true},
    {"zona":"Boton 1","densidad":12.5,"brillo_promedio":45.2,"es_ok":false}
  ]
}'
```

### Ejecutar unit tests

```bash
dotnet test tests/BitronCore.Tests/BitronCore.Tests.csproj
```

## Puertos expuestos

| Servicio | Puerto |
|---|---|
| API REST + SignalR | `5000` |
| PostgreSQL | `5432` |
| MQTT (TCP) | `1883` |
| MQTT (WebSocket) | `9001` |
