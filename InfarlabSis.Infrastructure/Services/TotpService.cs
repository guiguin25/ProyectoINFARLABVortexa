using OtpNet;

namespace InfarlabSis.Infrastructure.Services
{
    public class TotpService
    {
        /// <summary>
        /// GENERA LA CLAVE DEL AUTHENTICATOR
        /// </summary>
        public string GenerarClaveSecreta()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        /// <summary>
        /// GENERA EL QR
        /// </summary>
        public string GenerarUriQR(string claveSecreta, string emailUsuario)
        {
            return $"otpauth://totp/INFARLAB:{emailUsuario}?secret={claveSecreta}&issuer=INFARLAB&digits=6&period=30";
        }

        /// <summary>
        /// VALIDA EL CÓDIGO DE 6 DÍGITOS DE LA APP
        /// </summary>
        public bool ValidarCodigo(string claveSecreta, string codigoIngresado)
        {
            try
            {
                var key = Base32Encoding.ToBytes(claveSecreta);
                var totp = new Totp(key);
                // Ventana de tolerancia: ±1 período (30 segundos)
                return totp.VerifyTotp(codigoIngresado, out _, new VerificationWindow(previous: 1, future: 1));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// GENERA EL CÓDIGO DEL AUTHENTICATOR
        /// </summary>
        public string GenerarCodigoActual(string claveSecreta)
        {
            var key = Base32Encoding.ToBytes(claveSecreta);
            var totp = new Totp(key);
            return totp.ComputeTotp();
        }
    }
}