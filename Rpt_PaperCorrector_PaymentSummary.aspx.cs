﻿using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using ShoppingCart.BL;
using System.Web.UI.WebControls;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

public partial class Rpt_PaperCorrector_PaymentSummary : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ControlVisibility("Search");
            FillDDL_Division();
            FillDDL_AcadYear();
            ddlStandard.Items.Insert(0, "All");
            ddlStandard.SelectedIndex = 0;
        }
    }
    private void FillDDL_Division()
    {
        Label lblHeader_Company_Code = default(Label);
        lblHeader_Company_Code = (Label)Master.FindControl("lblHeader_Company_Code");

        Label lblHeader_User_Code = default(Label);
        lblHeader_User_Code = (Label)Master.FindControl("lblHeader_User_Code");

        Label lblHeader_DBName = default(Label);
        lblHeader_DBName = (Label)Master.FindControl("lblHeader_DBName");

        if (string.IsNullOrEmpty(lblHeader_User_Code.Text))
            Response.Redirect("Default.aspx");

        DataSet dsDivision = ProductController.GetAllActiveUser_Company_Division_Zone_Center(lblHeader_User_Code.Text, lblHeader_Company_Code.Text, "", "", "2", lblHeader_DBName.Text);
        BindDDL(ddlDivision, dsDivision, "Division_Name", "Division_Code");
        ddlDivision.Items.Insert(0, "Select Division");
        ddlDivision.SelectedIndex = 0;


    }
    private void FillDDL_AcadYear()
    {
        DataSet dsAcadYear = ProductController.GetAllActiveUser_AcadYear();
        BindDDL(ddlAcadYear, dsAcadYear, "Description", "Id");
        ddlAcadYear.Items.Insert(0, "Select");
        ddlAcadYear.SelectedIndex = 0;


    }
    private void BindDDL(DropDownList ddl, DataSet ds, string txtField, string valField)
    {
        if (ds != null)
        {
            if (ds.Tables.Count != 0)
            {
                ddl.DataSource = ds;
                ddl.DataTextField = txtField;
                ddl.DataValueField = valField;
                ddl.DataBind();
            }
        }
    }
    private void Show_Error_Success_Box(string BoxType, string Error_Code)
    {
        if (BoxType == "E")
        {
            Msg_Error.Visible = true;
            Msg_Success.Visible = false;
            lblerror.Text = ProductController.Raise_Error(Error_Code);
            UpdatePanelMsgBox.Update();
        }
        else
        {
            Msg_Success.Visible = true;
            Msg_Error.Visible = false;
            lblSuccess.Text = ProductController.Raise_Error(Error_Code);
            UpdatePanelMsgBox.Update();
        }
    }
    private void ControlVisibility(string Mode)
    {
        if (Mode == "Search")
        {
            DivResultPanel.Visible = false;
            DivSearchPanel.Visible = true;
            BtnShowSearchPanel.Visible = false;


        }
        else if (Mode == "Result")
        {
            DivResultPanel.Visible = true;
            DivSearchPanel.Visible = false;
            BtnShowSearchPanel.Visible = true;


        }

        Clear_Error_Success_Box();
    }
    private void Clear_Error_Success_Box()
    {
        Msg_Error.Visible = false;
        Msg_Success.Visible = false;
        lblSuccess.Text = "";
        lblerror.Text = "";
        UpdatePanelMsgBox.Update();
    }
    public decimal GetRateValue(object a)
    {

        decimal rate = 0;
        if (a.GetType().FullName != "System.DBNull")
        {
            rate = Convert.ToDecimal(a);
        }
        return rate;
    }


    private void FillDDL_Standard()
    {
        string Div_Code = null;
        Div_Code = ddlDivision.SelectedValue;

        string YearName = null;
        YearName = ddlAcadYear.SelectedItem.ToString();

        DataSet dsStandard = ProductController.GetAllActive_Standard_ForYear(Div_Code, YearName);
        BindDDL(ddlStandard, dsStandard, "Standard_Name", "Standard_Code");
        ddlStandard.Items.Insert(0, "All");
        ddlStandard.SelectedIndex = 0;
    }

    private void Fill_Grid()
    {
        try
        {


            //Validate if all information is entered correctly
            if (ddlDivision.SelectedIndex == 0)
            {
                Show_Error_Success_Box("E", "0001");
                ddlDivision.Focus();
                return;
            }


            if (ddlAcadYear.SelectedIndex == 0)
            {
                Show_Error_Success_Box("E", "0002");
                ddlAcadYear.Focus();
                return;
            }

            //if (ddlCourse.SelectedIndex == 0)
            //{
            //    Show_Error_Success_Box("E", "0003");
            //    ddlCourse.Focus();
            //    return;
            //}
            if (id_date_range_picker_1.Value == "")
            {
                Show_Error_Success_Box("E", "Select Date Range");
                id_date_range_picker_1.Focus();
                return;
            }



            //string Center_Code = "";
            //string Center_Name = "";
            //int CenterCnt = 0;
            //int CenterSelCnt = 0;




            //for (CenterCnt = 0; CenterCnt <= ddlcenter.Items.Count - 1; CenterCnt++)
            //{
            //    if (ddlcenter.Items[CenterCnt].Selected == true)
            //    {
            //        CenterSelCnt = CenterSelCnt + 1;
            //    }
            //}



            //if (CenterSelCnt == 0)
            //{
            //    //When all is selected   
            //    //Show_Error_Success_Box("E", "0006");
            //    ddlcenter.Focus();
            //    return;

            //}
            //else
            //{
            //    for (CenterCnt = 0; CenterCnt <= ddlcenter.Items.Count - 1; CenterCnt++)
            //    {
            //        if (ddlcenter.Items[CenterCnt].Selected == true)
            //        {
            //            Center_Code = Center_Code + ddlcenter.Items[CenterCnt].Value + ",";
            //            Center_Name = Center_Name + ddlcenter.Items[CenterCnt].Text + ",";
            //        }
            //    }
            //    Center_Code = Common.RemoveComma(Center_Code);
            //    Center_Name = Common.RemoveComma(Center_Name);

            //}

            //string Standard_Code = "";
            //string Standard_Name = "";
            //int standardCnt = 0;
            //int standardSelCnt = 0;

            //for (standardCnt = 0; standardCnt <= ddlCourse.Items.Count - 1; standardCnt++)
            //{
            //    if (ddlCourse.Items[standardCnt].Selected == true)
            //    {
            //        standardSelCnt = standardSelCnt + 1;
            //    }
            //}



            //if (standardSelCnt == 0)
            //{
            //    //When all is selected   
            //    //Show_Error_Success_Box("E", "0006");
            //    ddlCourse.Focus();
            //    return;

            //}
            //else
            //{
            //    for (standardCnt = 0; standardCnt <= ddlCourse.Items.Count - 1; standardCnt++)
            //    {
            //        if (ddlCourse.Items[standardCnt].Selected == true)
            //        {
            //            Standard_Code = Standard_Code + ddlCourse.Items[standardCnt].Value + ",";
            //            Standard_Name = Standard_Name + ddlCourse.Items[standardCnt].Text + ",";
            //        }
            //    }
            //    Standard_Code = Common.RemoveComma(Standard_Code);
            //    Standard_Name = Common.RemoveComma(Standard_Name);

            //}

            //string BatchCode = "";
            //string Batch_Name = "";
            //int BatchCnt = 0;
            //int BatchSelCnt = 0;
            //for (BatchCnt = 0; BatchCnt <= ddlBatch.Items.Count - 1; BatchCnt++)
            //{
            //    if (ddlBatch.Items[BatchCnt].Selected == true)
            //    {
            //        BatchSelCnt = BatchSelCnt + 1;
            //    }
            //}
            //if (BatchSelCnt == 0)
            //{
            //    //When all is selected   
            //    //Show_Error_Success_Box("E", "0006");
            //    ddlBatch.Focus();
            //    return;

            //}
            //else
            //{
            //    for (BatchCnt = 0; BatchCnt <= ddlCourse.Items.Count - 1; BatchCnt++)
            //    {
            //        if (ddlBatch.Items[BatchCnt].Selected == true)
            //        {
            //            BatchCode = Standard_Code + ddlBatch.Items[BatchCnt].Value + ",";
            //            Batch_Name = Standard_Name + ddlBatch.Items[BatchCnt].Text + ",";
            //        }
            //    }
            //    BatchCode = Common.RemoveComma(BatchCode);
            //    Batch_Name = Common.RemoveComma(Batch_Name);

            //}




            ControlVisibility("Result");
            string DivisionCode = null;
            DivisionCode = ddlDivision.SelectedValue;


            string AcademicYear = "";
            AcademicYear = ddlAcadYear.SelectedItem.Text;


            string CourseCode = "";
            CourseCode = ddlStandard.SelectedValue;


            string DateRange = "";
            DateRange = id_date_range_picker_1.Value;


            string FromDate, ToDate;
            FromDate = DateRange.Substring(0, 10);
            ToDate = (DateRange.Length > 9) ? DateRange.Substring(DateRange.Length - 10, 10) : DateRange;


            DateTime fdt = DateTime.ParseExact(FromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            DateTime tdt = DateTime.ParseExact(ToDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);


            lblDivision_Result.Text = ddlDivision.SelectedItem.ToString();
            //lblStandard_Result.Text = ddlCourse.SelectedItem.ToString();

            lblAcadYear_Result.Text = ddlAcadYear.SelectedItem.ToString();

            lblCourse.Text = ddlStandard.SelectedItem.ToString();
        
            //lblBatch.Text = Batch_Name;
            lblPeriod.Text = fdt.ToString("dd MMM yyyy") + " - " + tdt.ToString("dd MMM yyyy");

            DataSet dsGrid = null;
           dsGrid = ProductController.Get_RPTPaperCorrector_PaymentSummary(DivisionCode, AcademicYear, CourseCode, fdt, tdt);
            if (dsGrid != null)
            {
                if (dsGrid.Tables.Count != 0)
                {
                    if (dsGrid.Tables[0].Rows.Count != 0)
                    {
                        dlGridDisplay.DataSource = dsGrid;
                        dlGridDisplay.DataBind();
                        lbltotalcount.Text = dsGrid.Tables[0].Rows.Count.ToString();
                        // BtnAuthorization.Visible = true;
                        dlGridExport.DataSource = dsGrid;
                        dlGridExport.DataBind();
                    }
                    else
                    {
                        dlGridDisplay.DataSource = null;
                        dlGridDisplay.DataBind();
                        Show_Error_Success_Box("E", "Record not found ");
                        lbltotalcount.Text = "0";
                        // BtnAuthorization.Visible = false;
                        dlGridExport.DataSource = dsGrid;
                        dlGridExport.DataBind();
                    }
                }
                else
                {
                    dlGridDisplay.DataSource = null;
                    dlGridDisplay.DataBind();
                    Show_Error_Success_Box("E", "Record not found ");
                    lbltotalcount.Text = "0";
                    // BtnAuthorization.Visible = false;
                    dlGridExport.DataSource = dsGrid;
                    dlGridExport.DataBind();
                }

            }
            else
            {
                dlGridDisplay.DataSource = null;
                dlGridDisplay.DataBind();
                dlGridExport.DataSource = dsGrid;
                dlGridExport.DataBind();
                Show_Error_Success_Box("E", "Record not found ");
                lbltotalcount.Text = "0";
                //  BtnAuthorization.Visible = false;
            }

        }
        catch (Exception ex)
        {
            Msg_Error.Visible = true;
            Msg_Success.Visible = false;
            lblerror.Text = ex.ToString();
            UpdatePanelMsgBox.Update();
            //BtnAuthorization.Visible = false;
            return;

        }



    }
    protected void BtnSearch_Click(object sender, EventArgs e)
    {
        Fill_Grid();
    }
    protected void BtnShowSearchPanel_Click(object sender, EventArgs e)
    {
        ControlVisibility("Search");
    }
    protected void BtnClearSearch_Click(object sender, EventArgs e)
    {
        ddlDivision.SelectedIndex = 0;
        ddlAcadYear.SelectedIndex = 0;
        ddlStandard.SelectedIndex = 0;
       
        id_date_range_picker_1.Value = "";
        Clear_Error_Success_Box();
    }
    protected void HLExport_Click(object sender, EventArgs e)
    {


        dlGridExport.Visible = true;

        Response.Clear();
        Response.Buffer = true;
        Response.ContentType = "application/vnd.ms-excel";
        string filenamexls1 = "PaperCorrector_PaymentSummary_" + DateTime.Now + ".xls";
        Response.AddHeader("Content-Disposition", "inline;filename=" + filenamexls1);
        HttpContext.Current.Response.Charset = "utf-8";
        HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1250");
        //sets font
        HttpContext.Current.Response.Write("<font style='font-size:10.0pt; font-family:Calibri;'>");
        HttpContext.Current.Response.Write("<BR><BR><BR>");
        HttpContext.Current.Response.Write("<Table border='1'  borderColor='#000000' cellSpacing='0' cellPadding='0' style='font-size:10.0pt; font-family:Calibri; text-align:center;'> <TR style='color: #fff; background: black;text-align:center;'><TD Colspan='4'>Paper Corrector Payment Summary</b></TD></TR><TR style='color: #fff; background: black;text-align:left;'><TD Colspan='2'>Division - " + lblDivision_Result.Text + "</td><TD Colspan='2'>Academic Year - " + lblAcadYear_Result.Text + "</td></TR><TR style='color: #fff; background: black;text-align:left;'><TD Colspan='2'>Test Period - " + lblPeriod.Text + "</td><TD Colspan='2'>Course - " + lblCourse.Text + "</td></tr>");
        Response.Charset = "";
        this.EnableViewState = false;
        System.IO.StringWriter oStringWriter1 = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter oHtmlTextWriter1 = new System.Web.UI.HtmlTextWriter(oStringWriter1);
        //this.ClearControls(dladmissioncount)
        dlGridExport.RenderControl(oHtmlTextWriter1);
        Response.Write(oStringWriter1.ToString());
        Response.Flush();
        Response.End();


        dlGridExport.Visible = false;


    }
    protected void ddlAcadYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillDDL_Standard();
    }
    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillDDL_Standard();
    }
}