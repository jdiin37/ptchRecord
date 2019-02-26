using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PTCHRecordsys2
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    Session.Clear();
                    System.Web.Security.FormsAuthentication.SignOut();
                    Response.Redirect(ResolveUrl("~/Login.aspx"));
                }
            }
            catch (Exception ex)
            {
                //PublicLib.handleError("", this.GetType().Name, ex.Message);
            }

        }
    }
}