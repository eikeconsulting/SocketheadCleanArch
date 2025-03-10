using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SocketheadCleanArch.API.Authentication;

public static class JwtTokenRegistration
{
    public static IServiceCollection RegisterJwtTokenAuthentication(this IServiceCollection services, IConfiguration config)
    {
        IConfigurationSection section = config.GetSection("JwtTokenSettings");
        services.Configure<JwtTokenSettings>(section);
        var jwtTokenSettings = section.Get<JwtTokenSettings>();

        return services
            .AddAuthentication(configureOptions: c =>
            {
                c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(configureOptions: c =>
            {
                c.RequireHttpsMetadata = true;
                c.SaveToken = true;
                if (jwtTokenSettings is null)
                    return;
                c.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenSettings.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenSettings.Secret)),
                    ValidAudience = jwtTokenSettings.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            })
            .Services;
    }
    
}