using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;

namespace Sol_Demos.Extensions.Services;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HmacSignatureValidationServiceAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Read the request body
        // Clone the request body
        context.HttpContext.Request.EnableBuffering();

        context.HttpContext.Request.Body.Position = 0;
        //using var reader = new StreamReader(context.HttpContext.Request.Body);
        using var reader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.HttpContext.Request.Body.Position = 0;

        // string to byte array
        byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

        // Get the client ID from the headers
        var clientId = context.HttpContext.Request.Headers["X-CLIENT-ID"].ToString();

        // Fetch the secret key from the database using the client ID
        var secret = await GetSecretKeyFromDatabaseAsync(clientId);

        // Get the signature from the headers
        var clientSignature = context.HttpContext.Request.Headers["X-AUTH-SIGNATURE"].ToString();

        // Generate the server-side signature
        var serverSignature = GenerateSignature(bodyBytes, secret);

        // Compare signatures
        if (clientSignature == serverSignature)
        {
            // Process the order
            await next();
        }
        else
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private async Task<string> GetSecretKeyFromDatabaseAsync(string clientId)
    {
        // Implement your database access logic here to retrieve the secret key
        // For example, using Entity Framework or Dapper to query the database
        // This is a placeholder for demonstration purposes
        return await Task.FromResult("MySecretKey");
    }

    private string GenerateSignature(byte[] payload, string secret)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
        {
            byte[] hash = hmac.ComputeHash(payload);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}