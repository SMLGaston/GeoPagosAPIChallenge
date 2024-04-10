Challenge Payments GeoPagos

Para ejecutar este proyecto,realizado en .NET 8, es necesario utilizar Visual Studio 2022, Y Docker Desktop.

El archivo de apertura es GeoPagosAPI.sln

Para utilizar los endpoints se recomienda Postman.

Endpoints:
1)
POST http://localhost:5002/api/Autorizaciones

Para crear una autorización.

Esquema ejemplo:

{
    "ClienteId": 1,
    "TipoCliente": 2,
    "TipoAutorizacion": 1,
    "Monto": 10,
    "Estado" : 1
}

los decimales del monto deben escribirse con punto (ej 100.34) y no en coma.

2)
GET http://localhost:5002/api/Autorizaciones/Reporte 

Devuelve todas las autorizaciones creadas.

3)
GET http://localhost:5002/api/Autorizaciones/ReporteAprobadas

Devuelve todas las autorizaciones aprobadas.

4)
PUT http://localhost:5002/api/TablaReversas/Confirmar/{nro Autorizacion Id}

Confirma la autorizacion con el modelo, de no hacerlo se genera la autorizacion de reversa.

5)
GET http://localhost:5004/Procesador/{numero entero o decimal}

Test del servicio externo del procesador de pagos. Devuelve verdadero si es entero, falso si es decimal.


appsettings.json

En el apartado de Configuración, el item "TiempoConfirmacion" setea el tiempo en segundos requerido para no generar la autorización de reversa.
Su configuración por default es de 300 segundos (5 minutos).
