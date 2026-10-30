namespace Vista_usuario
{
    public static class ArmarMail
    {
        public static string DireccionCorreo { get; set; }
        public static string Asunto { get; set; }
        public static string NuevaContraseña { get; set; }
        public static string Usuario { get; set; }
        public static string CodigoVerificacion { get; set; }

        private const string ESTILO = "<style>h1{color:dodgerblue;} h2{color:darkorange;}</style>";

        public static void Preparar()
        {
            string body = ESTILO +
                "<h1>Datos de acceso</h1>" +
                "<p>Usuario: " + Usuario + "</p>" +
                "<h2>Contraseña: " + NuevaContraseña + "</h2>" +
                "<p>Es una contraseña temporal: al entrar por primera vez el sistema te va a pedir que elijas una propia.</p>";
            EnviarMail.sendMail(DireccionCorreo, Asunto, body);
        }

        public static void PrepararNuevaContraseña()
        {
            string body = ESTILO +
                "<h1>Recuperación de contraseña</h1>" +
                "<p>Hola " + Usuario + ", respondiste bien tus preguntas de seguridad.</p>" +
                "<h2>Tu contraseña nueva es: " + NuevaContraseña + "</h2>" +
                "<p>Cuando entres, el sistema te va a pedir que la cambies por una propia.</p>";
            EnviarMail.sendMail(DireccionCorreo, Asunto, body);
        }

        public static void PrepararCodigoIngreso()
        {
            string body = ESTILO +
                "<h1>Código de ingreso</h1>" +
                "<p>Hola " + Usuario + ", para terminar de iniciar sesión escribí este código:</p>" +
                "<h2>" + CodigoVerificacion + "</h2>";
            EnviarMail.sendMail(DireccionCorreo, Asunto, body);
        }
    }
}
