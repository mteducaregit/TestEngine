﻿using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using ShoppingCart.BL;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using System.Web;
using System.IO;
using System.Data.OleDb;
using System.Web.UI;
using System.Net.Mail;
using System.Net;

public partial class Rpt_MarkAuthentication_Detailed : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            FillDDL_Division();
            FillDDL_AcadYear();
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
        ddlDivision.Items.Insert(0, "Select");
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
        else if (Mode == "Manage")
        {

            DivResultPanel.Visible = false;
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
    private void FillDDL_Standard()
    {
        string Div_Code = null;
        Div_Code = ddlDivision.SelectedValue;

        string YearName = null;
        YearName = ddlAcadYear.SelectedItem.ToString();

        DataSet dsStandard = ProductController.GetAllActive_Standard_ForYear(Div_Code, YearName);
        BindDDL(ddlStandard, dsStandard, "Standard_Name", "Standard_Code");
        ddlStandard.Items.Insert(0, "Select");
        ddlStandard.SelectedIndex = 0;
    }
    private void FillDDL_Search_Centre()
    {
        Label lblHeader_Company_Code = default(Label);
        lblHeader_Company_Code = (Label)Master.FindControl("lblHeader_Company_Code");

        Label lblHeader_User_Code = default(Label);
        lblHeader_User_Code = (Label)Master.FindControl("lblHeader_User_Code");

        Label lblHeader_DBName = default(Label);
        lblHeader_DBName = (Label)Master.FindControl("lblHeader_DBName");

        string Div_Code = null;
        Div_Code = ddlDivision.SelectedValue;

        DataSet dsCentre = ProductController.GetAllActiveUser_Company_Division_Zone_Center(lblHeader_User_Code.Text, lblHeader_Company_Code.Text, Div_Code, "", "5", lblHeader_DBName.Text);

        BindListBox(ddlCentre, dsCentre, "Center_Name", "Center_Code");
        ddlCentre.Items.Insert(0, "");
        ddlCentre.Items.Insert(1, "All");
        //ddlCentre.SelectedIndex = 0;
    }
    private void BindListBox(ListBox ddl, DataSet ds, string txtField, string valField)
    {
        ddl.DataSource = ds;
        ddl.DataTextField = txtField;
        ddl.DataValueField = valField;
        ddl.DataBind();
    }
    protected void BtnClearSearch_Click(object sender, EventArgs e)
    {
        Clear();
    }
    protected void BtnSearch_Click(object sender, EventArgs e)
    {

        FillGrid();
    }
    private void FillGrid()
    {

        try
        {

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

            if (ddlStandard.SelectedIndex == 0)
            {
                Show_Error_Success_Box("E", "0003");
                ddlStandard.Focus();
                return;
            }
            if (ddlCentre.SelectedIndex == 0)
            {
                Show_Error_Success_Box("E", "select Center");
                ddlStandard.Focus();
                return;
            }

            if (id_date_range_picker_1.Value == "")
            {
                Show_Error_Success_Box("E", "Select Date Range");
                id_date_range_picker_1.Focus();
                return;
            }




            string Div_Code;
            Div_Code = ddlDivision.SelectedValue;

            string AcadYear;
            AcadYear = ddlAcadYear.SelectedItem.ToString();

            string course;
            course = ddlStandard.SelectedValue;

            string Center;
            Center = ddlCentre.SelectedValue;


            string DateRange = null;
            DateRange = id_date_range_picker_1.Value;

            if (string.IsNullOrEmpty(DateRange))
            {
                Show_Error_Success_Box("E", "0070");
                id_date_range_picker_1.Focus();
                return;
            }


            string FromDate, ToDate;
            FromDate = DateRange.Substring(0, 10);
            ToDate = (DateRange.Length > 9) ? DateRange.Substring(DateRange.Length - 10, 10) : DateRange;


            DateTime fdt = DateTime.ParseExact(FromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            DateTime tdt = DateTime.ParseExact(ToDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);


            string Center_Code = "";
            string Center_Name = "";
            int CenterCnt = 0;
            int CenterSelCnt = 0;

            for (CenterCnt = 0; CenterCnt <= ddlCentre.Items.Count - 1; CenterCnt++)
            {
                if (ddlCentre.Items[CenterCnt].Selected == true)
                {
                    CenterSelCnt = CenterSelCnt + 1;
                }
            }



            if (CenterSelCnt == 0)
            {
                //When all is selected   
                Show_Error_Success_Box("E", "0006");
                ddlCentre.Focus();
                return;

            }
            else
            {
                for (CenterCnt = 0; CenterCnt <= ddlCentre.Items.Count - 1; CenterCnt++)
                {
                    if (ddlCentre.Items[CenterCnt].Selected == true)
                    {
                        Center_Code = Center_Code + ddlCentre.Items[CenterCnt].Value + ",";
                        Center_Name = Center_Name + ddlCentre.Items[CenterCnt].Text + ",";
                    }
                }
                Center_Code = Common.RemoveComma(Center_Code);
                Center_Name = Common.RemoveComma(Center_Name);

            }




            lblDivision_Result.Text = ddlDivision.SelectedItem.ToString();
            //lblStandard_Result.Text = ddlStandard.SelectedItem.ToString();

            lblAcadYear_Result.Text = ddlAcadYear.SelectedItem.ToString();
            lblCentre_Result.Text = Center_Name;
            lblcourse.Text = ddlStandard.SelectedItem.ToString(); ;

            lblPeriod_Result.Text = fdt.ToString("dd MMM yyyy") + " - " + tdt.ToString("dd MMM yyyy");





            DataSet dsGrid = null;
            dsGrid = ProductController.GetAttendance_Mark_AuthorisationDetailed(Div_Code, AcadYear, course, Center_Code, fdt, tdt,2);

            if (dsGrid != null)
            {
                if (dsGrid.Tables.Count != 0)
                {
                    if (dsGrid.Tables[0].Rows.Count != 0)
                    {
                        Msg_Error.Visible = false;
                        DivSearchPanel.Visible = false;
                        DivResultPanel.Visible = true;
                        BtnShowSearchPanel.Visible = true;
                        dlGridDisplay.DataSource = dsGrid;
                        dlGridDisplay.DataBind();
                        lbltotalcount.Text = dsGrid.Tables[0].Rows.Count.ToString();
                        // BtnAuthorization.Visible = true;
                        //dlGridExport.DataSource = dsGrid;
                        //dlGridExport.DataBind();
                    }
                    else
                    {
                        dlGridDisplay.DataSource = null;
                        dlGridDisplay.DataBind();
                        Show_Error_Success_Box("E", "Records Not Found ");
                        // lbltotalcount.Text = "0";
                        // Show_Error_Success_Box("E", "Record not found ");
                        lbltotalcount.Text = "0";
                        // BtnAuthorization.Visible = false;
                        //dlGridExport.DataSource = null;
                        //dlGridExport.DataBind();
                    }
                }
                else
                {
                    dlGridDisplay.DataSource = null;
                    dlGridDisplay.DataBind();
                    // lbltotalcount.Text = "0";

                    lbltotalcount.Text = "0";
                    // BtnAuthorization.Visible = false;
                    //dlGridExport.DataSource = null;
                    //dlGridExport.DataBind();
                }

            }
            else
            {
                dlGridDisplay.DataSource = null;
                dlGridDisplay.DataBind();
                //dlGridExport.DataSource = null;
                //dlGridExport.DataBind();
                // lbltotalcount.Text = "0";
                // Show_Error_Success_Box("E", "Record not found ");
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
    protected void HLExport_Click(object sender, EventArgs e)
    {

        dlGridDisplay.Visible = true;

        Response.Clear();
        Response.Buffer = true;
        Response.ContentType = "application/vnd.ms-excel";
        string filenamexls1 = "Rpt_Markauyhorisation_Detailed_" + DateTime.Now + ".xls";
        Response.AddHeader("Content-Disposition", "inline;filename=" + filenamexls1);
        HttpContext.Current.Response.Charset = "utf-8";
        HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1250");
        //sets font
        HttpContext.Current.Response.Write("<font style='font-size:10.0pt; font-family:Calibri;'>");
        HttpContext.Current.Response.Write("<BR><BR><BR>");
        HttpContext.Current.Response.Write("<Table border='1'  borderColor='#000000' cellSpacing='0' cellPadding='0' style='font-size:10.0pt; font-family:Calibri; text-align:center;'> <TR style='color: #fff; background: black;text-align:center;'><TD Colspan='12'>Rpt Mark Authorisation Detailed</b></TD></TR><TR style='color: #fff; background: black;text-align:center;'><TD Colspan='1'>Division</td><TD Colspan='2'>" + lblDivision_Result.Text + "</td><TD Colspan='1'>Academic Year</td><TD Colspan='2'>" + lblAcadYear_Result.Text + "</td><TD Colspan='1'>Test Period</td><TD Colspan='5' style='color: #fff; background: black;text-align:left;'>" + lblPeriod_Result.Text + "</td></tr><TR style='color: #fff; background: black;text-align:center;'><TD Colspan='1'>Centre</td><TD Colspan='2' style='color: #fff; background: black;text-align:left;'>" + lblCentre_Result.Text + "</td><TD Colspan='1'>Course</td><TD Colspan='5' style='color: #fff; background: black;text-align:left;'>" + lblcourse.Text + "</td></tr>");
        Response.Charset = "";
        this.EnableViewState = false;
        System.IO.StringWriter oStringWriter1 = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter oHtmlTextWriter1 = new System.Web.UI.HtmlTextWriter(oStringWriter1);
        //this.ClearControls(dladmissioncount)
        dlGridDisplay.RenderControl(oHtmlTextWriter1);
        Response.Write(oStringWriter1.ToString());
        Response.Flush();
        Response.End();


        dlGridDisplay.Visible = false;
    }
    protected void BtnShowSearchPanel_Click(object sender, EventArgs e)
    {
        DivResultPanel.Visible = false;
        DivSearchPanel.Visible = true;

    }
    protected void ddlAcadYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillDDL_Standard();
        Clear_Error_Success_Box();
    }
    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillDDL_Standard();
        FillDDL_Search_Centre();
        Clear_Error_Success_Box();
    }
    protected void ddlStandard_SelectedIndexChanged(object sender, EventArgs e)
    {

        Clear_Error_Success_Box();
    }
    protected void ddlCentre_SelectedIndexChanged(object sender, EventArgs e)
    {

        Clear_Error_Success_Box();

    }
    protected void Clear()
    {
        ddlDivision.SelectedIndex = 0;
        ddlAcadYear.SelectedIndex = 0;
        ddlCentre.Items.Clear();

        ddlStandard.Items.Clear();
        id_date_range_picker_1.Value = "";

    }
}