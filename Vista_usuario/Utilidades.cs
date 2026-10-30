using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Vista_usuario
{
    public static class Utilidades
    {
        public static string HashSHA256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string HashContraseña(string contraseña, string usuario)
        {
            return HashSHA256(contraseña + usuario);
        }

        public static string GenerarContraseña()
        {
            const string letras = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            Random azar = new Random();
            string contraseña = "";

            for (int i = 0; i < 10; i++)
            {
                contraseña = contraseña + letras[azar.Next(letras.Length)];
            }

            return contraseña;
        }

        public static string GenerarCodigo()
        {
            Random azar = new Random();
            return azar.Next(100000, 999999).ToString();
        }

        public static string OcultarCorreo(string correo)
        {
            int arroba = correo.IndexOf('@');

            if (arroba <= 2)
            {
                return correo;
            }

            return correo.Substring(0, 2) + "***" + correo.Substring(arroba);
        }

        public static void AlternarVisibilidadTexto(CheckBox chk, TextBox txt)
        {
            txt.UseSystemPasswordChar = !chk.Checked;
        }

        public static void SoloNumeros(KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
