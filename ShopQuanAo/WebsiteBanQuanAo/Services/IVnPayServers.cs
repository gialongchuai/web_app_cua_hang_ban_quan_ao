﻿using WebsiteBanQuanAo.Models;
using System.Collections.Specialized;
using System.Web;

namespace WebsiteBanQuanAo.Services
{
    public interface IVnPayServers
    {
        VnPaymentResponseModel PaymentExecute(NameValueCollection collections, string hashSecret);
        string CreatePaymentUrl(HttpContextBase context, VnPaymentRequestModel model);
    }
}
