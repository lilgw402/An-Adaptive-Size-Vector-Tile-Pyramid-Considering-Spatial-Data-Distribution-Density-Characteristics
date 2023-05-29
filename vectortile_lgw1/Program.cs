using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

namespace vectortile_lgw1
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            esriLicenseStatus licenseStatus = esriLicenseStatus.esriLicenseUnavailable;
            IAoInitialize m_Aolnitialize = new AoInitialize();
            licenseStatus = m_Aolnitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngine);
            // licenseStatus = m_Aolnitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngine);
            //  licenseStatus = m_Aolnitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeBasic);
            //licenseStatus = m_Aolnitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard);
            //licenseStatus = m_Aolnitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
            //licenseStatus = m_Aolnitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeArcServer);
            licenseStatus = m_Aolnitialize.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);
            licenseStatus = m_Aolnitialize.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst);
            Application.Run(new Form1());
        }
    }
}
