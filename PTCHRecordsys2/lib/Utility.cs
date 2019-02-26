using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public static class Utility
{
    #region AlertBox
    public static void showAlert(string Message)
    {
        try
        {
            System.Web.UI.Page page = HttpContext.Current.CurrentHandler as System.Web.UI.Page;
            string strScript = string.Format("showAlert('{0}');", Message);
            if (page != null && !page.ClientScript.IsClientScriptBlockRegistered("showAlert"))
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), "showAlert", strScript, true /* addScriptTags */);
            }
        }
        catch (Exception ex)
        {
            //PublicLib.handleError("", "PublicLib", ex.Message);
        }
    }

    public static void showAlert(string Message, string strUrl)
    {
        try
        {
            System.Web.UI.Page page = HttpContext.Current.CurrentHandler as System.Web.UI.Page;
            string strScript = string.Format("showAlertRedirect('{0}', '{1}');", Message, strUrl);
            if (page != null && !page.ClientScript.IsClientScriptBlockRegistered("alertMsg"))
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), "alertMsg", strScript, true /* addScriptTags */);
            }
        }
        catch (Exception ex)
        {
            //PublicLib.handleError("", "PublicLib", ex.Message);
        }
    }
    #endregion

}
