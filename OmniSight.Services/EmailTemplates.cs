namespace OmniSight.Services
{
    public static class EmailTemplates
    {
        public static string BuildActionEmailHtml(string appName, string title, string greetingName, string messageLine1, string buttonText, string actionUrl, int expiryMinutes, string fallbackUrl, string deepLink)
        {
            return $@"<html><body><h2>{title}</h2><p>Xin chào {greetingName},</p><p>{messageLine1}</p><p><a href='{actionUrl}' style='padding:10px 20px;background:#1976d2;color:#fff;text-decoration:none;border-radius:4px'>{buttonText}</a></p><p>Nếu nút không hoạt động, hãy copy link này vào trình duyệt:<br><a href='{fallbackUrl}'>{fallbackUrl}</a></p><p>Liên kết này sẽ hết hạn sau {expiryMinutes} phút.</p><p>Trân trọng,<br>{appName} Team</p></body></html>";
        }
    }
}
