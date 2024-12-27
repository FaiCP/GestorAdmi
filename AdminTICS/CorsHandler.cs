using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class CorsHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Respuesta para solicitudes preflight (OPTIONS)
        if (request.Method == HttpMethod.Options)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept, Authorization");
            return Task.FromResult(response);
        }


        // Para todas las demás solicitudes
        var task = base.SendAsync(request, cancellationToken);
        task.ContinueWith(t =>
        {
            if (t.Result != null)
            {
                t.Result.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
            }
        }, cancellationToken);

        return task;
    }
}
