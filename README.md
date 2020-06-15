# Fiive Web API 
![Fiive](https://fiivestudio.com/wp-content/uploads/2020/06/Fiive-Open-Source_2.png)

Esta solución es un API que contiene parte de la lógica para la solución **[Sports Play]([https://fiivestudio.com/2020/06/09/conoce-sports-play/](https://fiivestudio.com/2020/06/09/conoce-sports-play/))**. Esta Api fue construida basada en tokens y se conecta al servidor de autenticación para tener la autorización y poder acceder a los datos. 

Esta solución hace parte del proyecto **[Sports Play]([https://fiivestudio.com/2020/06/09/conoce-sports-play/](https://fiivestudio.com/2020/06/09/conoce-sports-play/))** y corresponde a la tercera aplicación de las tres que componen el proyecto. 

## Comenzando 🚀

A continuación describimos brevemente los pasos para colocar en funcionamiento el proyecto. 

### Pre-requisitos 📋

 - Framework 4.5.2 o superior. 
 - EntityFramework 	6.0
 - SQL server 2017.
   
### Instalación 🔧

 1. Descargar el proyecto del repositorio.
 2. Abrir el proyecto con el IDE de Visual Studio. 
 3. Actualizar la cadena de conexión de la base de datos en el archivo **Web.config**.
```
<connectionStrings>
    <add name="FutPlayAppDB" connectionString="data source=[SERVIDOR];initial catalog=sportsplay;persist security info=True;user id=[USER_DATABASE];password=[PASSWORD_DATABASE];MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" />
  </connectionStrings>
```
 4. Actualizar el servidor de correo para el envío de notificaciones. Para esto, abra el archivo (***FutbolPlay\Functions\CommonFunctions.cs***)

```
Linea 105: var fromAddress = new MailAddress("[CORREO_ELECTRONICO]", "[NOMBRE DE QUIEN ENVIA EL CORREO]");
Linea 107: const string fromPassword = "[CONTRASEÑA]";           
Linea 113: Host = "[SERVER DE CORREO SMTP]",
```

> **NOTA:** Repita el procedimiento anterior para las lineas 156, 158 y 164.

5. Compile y use.

## Construido con 🛠️

* [ASP.NET _Web API](https://dotnet.microsoft.com/apps/aspnet/apis) - Framework
* [Json](https://www.nuget.org/packages/Newtonsoft.Json/) - Formato para intercambio de datos.
* [Owin](http://owin.org/) - Interfaz entre aplicaciones web .NET y servidores web.
* [OAuth 2.0](https://oauth.net/2/) - Protocolo de autorización.

## Autores ✒️

* **[Alejandra Morales](https://fiivestudio.com/alejandra-morales)**
* **[Pablo Díaz](https://fiivestudio.com/pablo-diaz)**

## Notas Adicionales

* Tenga en cuenta que este proyecto es el tercero de los tres requeridos para que toda la solución de **[Sports Play]([https://fiivestudio.com/2020/06/09/conoce-sports-play/](https://fiivestudio.com/2020/06/09/conoce-sports-play/))** funcione correctamente. 
* Actualmente nos encontramos creando el Wiki detallado de la solución. 
