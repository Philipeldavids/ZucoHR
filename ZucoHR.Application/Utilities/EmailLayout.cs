using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Application.Utilities
{
    public static class EmailLayout
    {
        public static string Build(
            string title,
            string body
        )
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8' />
            </head>

            <body style='margin:0;padding:0;background:#f4f7fb;font-family:Arial,sans-serif;'>

                <table width='100%' cellpadding='0' cellspacing='0'>
                    <tr>
                        <td align='center' style='padding:40px 20px;'>

                            <table width='600' cellpadding='0' cellspacing='0'
                                   style='background:#ffffff;border-radius:12px;overflow:hidden;'>

                                <tr>
                                    <td style='background:#2563eb;padding:24px;text-align:center;color:#fff;'>

                                        <h1 style='margin:0;font-size:24px;'>
                                            ZucoHR
                                        </h1>

                                    </td>
                                </tr>

                                <tr>
                                    <td style='padding:32px;'>

                                        <h2 style='margin-top:0;color:#111827;'>
                                            {title}
                                        </h2>

                                        {body}

                                    </td>
                                </tr>

                                <tr>
                                    <td style='padding:20px;text-align:center;
                                               background:#f9fafb;
                                               color:#6b7280;
                                               font-size:12px;'>

                                        © {DateTime.UtcNow.Year} ZucoHR.
                                        All rights reserved.

                                    </td>
                                </tr>

                            </table>

                        </td>
                    </tr>
                </table>

            </body>
            </html>
            ";
        }
    }
}
