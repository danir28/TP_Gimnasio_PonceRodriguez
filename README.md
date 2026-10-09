# Sistema de Gestión de Gimnasio

Aplicación de escritorio en Windows Forms para administrar un gimnasio o club.
Gestiona **socios**, **planes** (a cargo de un **profesor**), **suscripciones**
de cada socio a un plan y los **pagos** de cada suscripción. Reemplaza el
control manual en planillas o cuadernos y permite saber quién tiene la cuota
vencida, cuánto se recaudó y cuántos socios tiene cada plan.

## Objetivos y funcionalidades

* Llevar el registro de los socios.
* Controlar los vencimientos de las suscripciones.
* Asignar un profesor a cada plan.
* Registrar los pagos de cada suscripción y consultar la recaudación.

### ABM de Socio

* **Alta:** el administrador registra un socio nuevo con nombre, apellido, DNI y teléfono.
* **Baja:** el administrador da de baja a un socio, que pasa a figurar como inactivo.
* **Modificación:** el administrador corrige los datos de un socio existente.
* **Consulta:** se listan los socios y se busca uno por DNI.

### ABM de Plan

* **Alta:** se crea un plan con nombre, precio y profesor a cargo.
* **Baja:** se desactiva un plan que ya no se ofrece.
* **Modificación:** se cambia el precio, el nombre o el profesor de un plan.
* **Consulta:** se listan los planes con su profesor.

## Reportes

1. **Socios con cuota vencida:** lista los socios cuya suscripción ya venció.
2. **Ingresos por mes:** total recaudado en pagos, agrupado por mes.
3. **Cantidad de socios por plan:** cuántos socios tiene cada plan.
4. **Historial de pagos de un socio:** todos los pagos de un socio, con fecha y monto.
5. **Altas del último mes:** socios registrados en los últimos 30 días.

## Integración de las capas

El proyecto se divide en una biblioteca de clases y una aplicación WinForms.
Para guardar un registro, por ejemplo un socio nuevo:

1. El administrador completa el formulario en **WinForms** y presiona "Guardar".
2. El formulario crea un objeto `Socio` con los datos ingresados y se lo pasa al **repositorio**.
3. El repositorio, que vive en la biblioteca de clases, usa el **DbContext** de Entity Framework Core para agregar el objeto y llamar a `SaveChanges()`.
4. Entity Framework Core traduce la operación a SQL y la guarda en la **base de datos**.

De esta forma la interfaz no conoce los detalles de la base de datos: solo habla con el repositorio.