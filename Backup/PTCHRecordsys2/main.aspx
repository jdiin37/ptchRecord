<%@ Page Title="" Language="C#" MasterPageFile="~/mainMaster.Master" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="PTCHRecordsys2.main" %>
<%@ MasterType VirtualPath="~/mainMaster.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
    #table_opdorder th,#table_opdorder td,#table_Statement th,#table_Statement td,#table_ipdorder th,#table_ipdorder td
    {
        text-align:center;
    }
    .tab-pane
    {
       border-bottom:1px solid #ddd;
       border-left:1px solid #ddd;
       border-right:1px solid #ddd;
       border-radius: 0px 0px 4px 4px;
       padding:5px;
    }
    .tab-content
    {
        background-color:White;
        
    }
        
</style>
<script>
//    function aspbtnClick(btnId) {
//        __doPostBack('btnClick', btnId);
//    }
    function ActiveTab(tabID) {
        $('.nav-tabs a[href="#' + tabID + '"]').tab('show');
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    

    <ul class="nav nav-tabs" id="tab_sum">
      <li id="tab_PatOther" runat="server" onclick="javascript:__doPostBack('','PatOther')"><a data-toggle="tab" href="#asp_panelPatOther">病人相關資料</a></li>
      <li id="tab_VisitRecord" runat="server" onclick="javascript:__doPostBack('','VisitRecord')"><a data-toggle="tab" href="#asp_panelVisitRecord">門診紀錄</a></li>
      <li id="tab_IpdOrder" runat="server" onclick="javascript:__doPostBack('','IpdOrder')"><a data-toggle="tab" href="#asp_panelIpdOrder">住院醫囑</a></li>
      <li id="tab_IpdInNote" runat="server" onclick="javascript:__doPostBack('','IpdInNote')"><a data-toggle="tab" href="#asp_panelIpdInNote">入院摘要</a></li>
      <li id="tab_IpdOutNote" runat="server" onclick="javascript:__doPostBack('','IpdOutNote')"><a data-toggle="tab" href="#asp_panelIpdOutNote">出院摘要</a></li>
      <li id="tab_IpdDiag" runat="server" onclick="javascript:__doPostBack('','IpdDiag')"><a data-toggle="tab" href="#asp_panelIpdDiag">診斷與處置</a></li>
      <li id="tab_Consultation" runat="server" onclick="javascript:__doPostBack('','Consultation')"><a data-toggle="tab" href="#asp_panelConsultation">會診</a></li>
      <li id="tab_ProgressNote" runat="server" onclick="javascript:__doPostBack('','ProgressNote')"><a data-toggle="tab" href="#asp_panelProgressNote">Progress Note</a></li>
      <li id="tab_Statement" runat="server" onclick="javascript:__doPostBack('','Statement')"><a data-toggle="tab" href="#asp_panelStatement">敘述醫囑</a></li>
      <li id="tab_LabReport" runat="server" onclick="javascript:__doPostBack('','LabReport')"><a data-toggle="tab" href="#asp_panelLabReport">檢查檢驗報告資料</a></li>
    </ul>

    <div class="tab-content">
        <asp:Panel ID="asp_panelPatOther" runat="server" ClientIDMode="Static" 
             CssClass="tab-pane fade" Enabled="False">  
                 <div class="form-group">
                    <div class="pre-scrollable well well-sm scrolldiv">
                        <asp:Literal ID="asp_LitPatOther" runat="server"></asp:Literal>
                    </div>
                 </div>
        </asp:Panel>
        <asp:Panel ID="asp_panelVisitRecord" runat="server" ClientIDMode="Static" 
            CssClass="tab-pane fade" Enabled="False">      
   
            <div class="form-group">
                <p>摘要</p>
                <div class="pre-scrollable well well-sm">
                    <asp:Label ID="asp_lbVisitRecord_memo" runat="server" Text="" CssClass="text-primary"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <p>主診斷 
                    <asp:Label ID="asp_lbVisitRecord_diagM" runat="server" Text="" CssClass="text-primary"></asp:Label>
                </p>
                <p>次診斷1
                    <asp:Label ID="asp_lbVisitRecord_diag1" runat="server" Text="" CssClass="text-primary"></asp:Label>
                </p>
                <p>次診斷2
                    <asp:Label ID="asp_lbVisitRecord_diag2" runat="server" Text="" CssClass="text-primary"></asp:Label>
                </p>
                <p>次診斷3
                    <asp:Label ID="asp_lbVisitRecord_diag3" runat="server" Text="" CssClass="text-primary"></asp:Label>
                </p>
            </div>
            <div class="form-group">
                <table id="table_opdorder" class="table table-bordered table-hover">
                    <thead>
                      <tr>
                        <th>險別</th>
                        <th>醫令碼</th>
                        <th>醫令名稱</th>
                        <th>劑量</th>
                        <th>單位</th>
                        <th>頻次</th>
                        <th>途徑</th>
                        <th>日</th>
                        <th>總量</th>
                        <th>單位</th>
                        <th>急</th>
                        <th>計</th>
                      </tr>
                    </thead>
                    <tbody>
                        <asp:Literal ID="asp_LitOpdOrder" runat="server"></asp:Literal>
                    </tbody>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel ID="asp_panelIpdOrder" runat="server" ClientIDMode="Static" 
            CssClass="tab-pane fade" Enabled="False">
            <div class="form-group">  
                <table id="table_ipdorder" class="table table-bordered table-hover">
                    <thead>
                        <tr>
                          <th>險別</th>
                          <th>醫令碼</th>
                          <th>醫令名稱</th>
                          <th>劑量</th>
                          <th>單位</th>
                          <th>頻次</th>
                          <th>服法</th>
                          <th>總量</th>
                          <th>日數</th>
                          <th>起服日</th>
                          <th>停服日</th>
                          <th>磨粉</th>
                          <th>類別說明</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Literal ID="asp_LitIpdOrder" runat="server"></asp:Literal>
                    </tbody>
                </table>  
            </div>
        </asp:Panel>
        <asp:Panel ID="asp_panelIpdInNote" runat="server" ClientIDMode="Static" 
            CssClass="tab-pane fade" Enabled="False">  
            <div class="form-group">
                <div class="pre-scrollable well well-sm scrolldiv">
                    <asp:Literal ID="asp_LitIpdInNote" runat="server"></asp:Literal>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="asp_panelIpdOutNote" runat="server" ClientIDMode="Static" 
            CssClass="tab-pane fade" Enabled="False"> 
            <div class="form-group">
                <div class="pre-scrollable well well-sm scrolldiv">
                    <asp:Literal ID="asp_LitIpdOutNote" runat="server"></asp:Literal>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="asp_panelIpdDiag" runat="server" ClientIDMode="Static" 
            CssClass="tab-pane fade" Enabled="False">  
                <div class="form-group">
                    <div class="pre-scrollable well well-sm scrolldiv">
                        <asp:Literal ID="asp_LitIpdDiag" runat="server"></asp:Literal>
                    </div>
                </div>
        </asp:Panel>
        <asp:Panel ID="asp_panelConsultation" runat="server" ClientIDMode="Static" 
            CssClass="tab-pane fade" Enabled="False"> 
            <div class="form-group">
                <asp:Literal ID="asp_LitConsultation" runat="server"></asp:Literal>
            </div>
        </asp:Panel>
        <asp:Panel ID="asp_panelProgressNote" runat="server" ClientIDMode="Static" 
            CssClass="tab-pane fade" Enabled="False">
            <span class="pull-right"><asp:Button ID="asp_btnProgressLink" runat="server" 
                Text="開啟ProgressNote程式" CssClass="btn btn-default" /></span>
            <div class="form-group">
                <asp:Literal ID="asp_LitProgressNote" runat="server"></asp:Literal>
            </div>
        </asp:Panel>
        <asp:Panel ID="asp_panelStatement" runat="server" ClientIDMode="Static" 
            CssClass="tab-pane fade" Enabled="False">    
            <div class="form-group">  
                <table id="table_Statement" class="table table-bordered table-hover">
                    <thead>
                        <tr>
                          <th>類別</th>
                          <th>起始日</th>
                          <th>醫令</th>
                          <th>頻次</th>
                          <th>停止日</th>
                          <th>院碼</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Literal ID="asp_LitStatementOrder" runat="server"></asp:Literal>
                    </tbody>
                </table>  
            </div>
        </asp:Panel>
        <asp:Panel ID="asp_panelLabReport" runat="server" ClientIDMode="Static" 
            CssClass="tab-pane fade" Enabled="False">
            <div class="form-group">
                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
            </div>
            <div class="btn-group btn-group-justified" role="group" aria-label="...">
                <div class="btn-group" role="group">
                    <asp:Button ID="btn_LabReport_Exam" runat="server" Text="檢驗" 
                        CssClass="btn btn-default"/>
                </div>
                <div class="btn-group" role="group">
                    <asp:Button ID="btn_LabReport_ExamSum" runat="server" Text="檢驗彙總" 
                        CssClass="btn btn-default"/>
                </div>
                <div class="btn-group" role="group">
                    <asp:Button ID="btn_LabReport_Xray" runat="server" Text="放射報告" 
                        CssClass="btn btn-default"/>
                </div>
                <div class="btn-group" role="group">
                    <asp:Button ID="btn_LabReport_Microbe" runat="server" Text="微生物" 
                        CssClass="btn btn-default"/>
                </div>
                <div class="btn-group" role="group">
                    <asp:Button ID="btn_LabReport_Inspect" runat="server" Text="檢查" 
                        CssClass="btn btn-default"/>
                </div>
                <div class="btn-group" role="group">
                    <asp:Button ID="btn_LabReport_Semen" runat="server" Text="精液分析" 
                        CssClass="btn btn-default"/>
                </div>
                <div class="btn-group" role="group">
                    <asp:Button ID="btn_LabReport_Pathology" runat="server" Text="病理報告" 
                        CssClass="btn btn-default"/>
                </div>
                <div class="btn-group" role="group">
                    <asp:Button ID="btn_LabReport_Surgery" runat="server" Text="手術" 
                        CssClass="btn btn-default"/>
                </div>
                <div class="btn-group" role="group">
                    <asp:Button ID="btn_LabReport_EKG" runat="server" Text="EKG" 
                        CssClass="btn btn-default"/>
                </div>
                <div class="btn-group" role="group">
                    <asp:Button ID="btn_LabReport_Tumor" runat="server" Text="腫瘤科報告" 
                        CssClass="btn btn-default"/>
                </div>
                <div class="btn-group" role="group">
                    <asp:Button ID="btn_LabReport_Special" runat="server" Text="特殊表單" 
                        CssClass="btn btn-default"/>
                </div>
            </div>
        </asp:Panel>
    </div>

    <script>
    </script>
</asp:Content>
