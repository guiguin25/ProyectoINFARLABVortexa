using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace InfarlabSis.Infrastructure.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarCorreoRecuperacion(string emailDestino, string enlaceRecuperacion)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPass = _configuration["EmailSettings:SmtpPass"];
                var remitente = _configuration["EmailSettings:Remitente"] ?? "INFARLAB";

                using var cliente = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(smtpUser, remitente),
                    Subject = "Recuperación de Contraseña - INFARLAB",
                    IsBodyHtml = true,
                    Body = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <div style='background: linear-gradient(135deg, #0284c7, #0369a1); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
                                <h1 style='color: white; margin: 0;'>INFARLAB</h1>
                                <p style='color: #e0f2fe; margin: 10px 0 0 0;'>Laboratorio Farmacéutico Industrial</p>
                            </div>
                            
                            <div style='background: #f8fafc; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e2e8f0;'>
                                <h2 style='color: #0f172a; margin-top: 0;'> Solicitud de Recuperación de Contraseña</h2>
                                
                                <p style='color: #475569; line-height: 1.6;'>
                                    Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.
                                </p>
                                
                                <p style='color: #475569; line-height: 1.6;'>
                                    Haz clic en el siguiente botón para crear una nueva contraseña:
                                </p>
                                
                                <div style='text-align: center; margin: 30px 0;'>
                                    <a href='{enlaceRecuperacion}' 
                                       style='background: linear-gradient(135deg, #0284c7, #0369a1); 
                                              color: white; 
                                              padding: 14px 32px; 
                                              text-decoration: none; 
                                              border-radius: 8px; 
                                              font-weight: bold;
                                              display: inline-block;'>
                                        Restablecer Contraseña
                                    </a>
                                </div>
                                
                                <p style='color: #64748b; font-size: 13px; line-height: 1.6;'>
                                    <strong>Importante:</strong> Este enlace expirará en <strong>1 hora</strong> por seguridad.
                                </p>
                                
                                <p style='color: #64748b; font-size: 13px; line-height: 1.6;'>
                                    Si no solicitaste este cambio, puedes ignorar este correo con total seguridad.
                                </p>
                                
                                <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 30px 0;'>
                                
                                <p style='color: #94a3b8; font-size: 12px; text-align: center;'>
                                    © 2026 INFARLAB - Sistema de Gestión Farmacéutica y Logística
                                </p>
                            </div>
                        </div>
                    "
                };

                mensaje.To.Add(emailDestino);

                await cliente.SendMailAsync(mensaje);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al enviar correo: {ex.Message}");
            }
        }
    }
}