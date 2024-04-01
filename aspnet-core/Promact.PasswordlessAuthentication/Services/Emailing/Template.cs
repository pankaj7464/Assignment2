namespace Promact.PasswordlessAuthentication.Services.Emailing
{
    public static class Template
    {
        public static string css = $@"<style>
      /* CSS styles can be added here to style the email content */
      body {{
          font-family: Arial, sans-serif;
          line-height: 1.6;
      }}

      .container {{
          margin: 0 auto;
          padding: 20px;
          border: 1px solid #ccc;
          border-radius: 5px;
      }}

      .btn {{
          display: inline-block;
          background-color: #007bff;
          color: #fff;
          padding: 10px 20px;
          text-decoration: none;
          border-radius: 5px;
      }}

      .btn:hover {{
          background-color: #0056b3;
      }}
  </style>";

        internal static string GetEmailTemplate(string name, string loginLink)
        {
            return $@"
    <html>
    <head>
        <title>Login to Your Account</title>
        {css}
    </head>
    <body>
        <div class='container'>
            <h2>Hello {name},</h2>
            <p>You have requested a login link for accessing your account on our website. Please use the following link to log in:</p>
            <p><a href='{loginLink}' class='btn'>Login to Your Account (One-Time Link)</a></p>
            <p>If you did not request this, you can safely ignore this email.</p>
            <p>Thank you,</p>
            <p>Promact Team</p>
        </div>
    </body>
    </html>";
        }

    }
}
