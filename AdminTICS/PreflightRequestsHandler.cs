using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class PreflightRequestsHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Verificar si la solicitud es de tipo OPTIONS
        if (request.Method == HttpMethod.Options)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            // Agregar cabeceras necesarias para CORS
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

            return Task.FromResult(response);
        }

        return base.SendAsync(request, cancellationToken);
    }
}