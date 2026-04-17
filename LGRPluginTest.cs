using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tekla.Structures;
using Tekla.Structures.Internal;
using Tekla.Structures.Model;
using Tekla.Structures.Plugins;
using T3D = Tekla.Structures.Geometry3d;
using TSDT = Tekla.Structures.Datatype;
using TSM = Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;
using TSO = Tekla.Structures.Model.Operations;
using TSP = Tekla.Structures.Plugins;

namespace LGRPluginTest
{

    #region Data Passed From UI to Plugin

    // the "height" is the AttributeName of the TextBox for example

    public class StructuresData
    {
        [TSP.StructuresField("height")]
        public double height;
    }

    #endregion


    #region Plugin Description
    // Description of this plugin.
    // First the plugin name
    // then the UI for the plugin
    // after that for translation purposes
    [TSP.Plugin("LGRPluginTest")]
    [TSP.PluginUserInterface("LGRPluginTest.UserControl1")]
    [TSP.PluginDescription("enu", "LGRPluginTest")]
    [TSP.PluginCoordinateSystem(PluginBase.CoordinateSystemType.FROM_FIRST_AND_SECOND_POINT)]
    //This one would be where the insertion point of the plugin changed, the plugin would re-run
    [TSP.InputObjectDependency(InputObjectDependency.DEPENDENT)]
    // [TSP.InputObjectDependency(InputObjectDependency.NOT_DEPENDENT_MODIFIABLE)]

    #endregion


    #region Plugin Definition

    // Here is where we are actually defining the Tekla Structures plugin class
    // LGRPluginTest is the plugin name
    // PluginBase because is model, not drawing
    public class LGRPluginTest : TSP.PluginBase
    {

        #region Fields

        // Enables inserting of objects in a model.
        private readonly TSM.Model _model;
        // Enables retrieving of input values.
        private readonly StructuresData _data;

        // private int NumberOfBeams = 5;


        #endregion

        public TSM.Model Model
        {
            get { return _model; }
        }

        #region Constructor

        public LGRPluginTest(StructuresData data)
        {
            // Link to model.
            _model = new TSM.Model();

            // Link to input values.
            _data = data;
        }

        #endregion


        #region Inputs method that gets user to pick points and parts used in the plugin


        // Specifies the user input needed for the plugin.
        // <returns>List of input definitions</returns>
        public override List<InputDefinition> DefineInput()
        {
            // Define input objects.

            TSMUI.Picker PointPicker = new TSMUI.Picker();

            List<InputDefinition> PointList = new List<InputDefinition>();

            T3D.Point InputPoint = PointPicker.PickPoint();

            InputDefinition InputDef = new InputDefinition(InputPoint);

            PointList.Add(InputDef);

            return PointList;
        }


        #endregion


        #region Run Method that executes the code

        // This function is called upon execution of the plugin.
        // <param name="input">List of input definitions</param>
        // <returns></returns>
        public override bool Run(List<InputDefinition> input)
        {
            try
            {
                // Write your code here.

                // Gets values from the StructuresData class and sets them to the fields in the plugin
                // If fields left empty on dialog, the hard coded defaults are set.

                // For example: GetValuesFromDialog();

                double Height = _data.height;

                // Get picked point in Tekla Structures
                T3D.Point StartPoint = (T3D.Point)input[0].GetInput();

                // Calculate the end point of the beam
                T3D.Point EndPoint = new T3D.Point(StartPoint);
                EndPoint.Z += Height;

                // Change the workplane further, or do something with the pickpoints if needed

                // double PickedPointDistance = T3D.Distance.PointToPoint(StartPoint, EndPoint);


                // Your Execution Code Goes Here!

                // A good practice is to have the class in a separate .cs to be reused on other plugin or app. Below is not the ideal...
                // CreateBuilding(PickedPointDistance);

                // Create a beam instance
                TSM.Beam Column = new TSM.Beam(StartPoint, EndPoint);
                Column.Profile.ProfileString = "HEA400";

                // Insert the beam in the model
                Column.Insert();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return true;
        }


        #endregion


        #region Get Values from dialog (example) --> GetValuesFromDialog()

        //private void GetValuesFromDialog()
        //{
        //    if (true)
        //    {

        //    }
        //    else
        //    {

        //    }
        //}


        #endregion


        #region Reads the Project Data from Data File in MBS Job or Tekla Model folder

        //private void FetchProjectData()
        //{

        //}


        #endregion


        #region Create The Building


        //public void CreateBuilding(double PickedPointDistance)
        //{
        // Get number, spacing, and profile size of Purlin data from Data file
        //FetchProjectData();



        // DON'T CALL COMMIT CHANGES. PLUGINS HANDLE THIS INTERNALLY
        //Model.CommitChanges();

        //}

        #endregion


        #region Create Purlin

        //public Beam CreatePurlin(Point StartPoint, Point EndPoint, string ProfileSize, string MBSMarkNumber)
        //{
        //    Beam Purlin = new Beam();
        //    Purlin.StartPoint = StartPoint;
        //    Purlin.StartPointOffset.Dx = 0;
        //    // Tekla develore said Label is meant to be unique
        //    // However, this isn't documented in the API Reference online

        //    //Purlin.SetLabel("ROOF-" + MBSMarkNumber);
        //    //if (Purlin.Insert())
        //    //{

        //    //    // I have seen issues with UDAs being set vs. not set on other objects in plugins
        //    //    // and whether Tekla assigned an existing ID toa different part during a modify
        //    //    // UDAs set from the initial insertion / previos modify, that aren't set to a new object
        //    //    // can carry incorrect residual UDA values over to another object.

        //    //    Purlin.SetUserProperty("MbsPurpose", "ROOF");
        //    //}
        //    //return Purlin;

        //}

        #endregion

    }

    #endregion

}