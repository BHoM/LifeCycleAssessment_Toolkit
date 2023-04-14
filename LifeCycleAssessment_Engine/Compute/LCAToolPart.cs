/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Structure.MaterialFragments;
using System.IO;
using System.Text;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        [Description("The LCA_tool_part_tostart.py script in c#.")]
        [Input("filePath", ".")]
        [Input("c2_input", ".")]
        [Input("d2_input", ".")]
        [Input("b2_input", ".")]
        [Input("e2_input", ".")]
        [Input("g2_input", ".")]
        [Input("h2_input", ".")]
        [Input("f2_input", ".")]
        [Input("o2_input", ".")]
        [Input("i2_input", ".")]
        [Input("j2_input", ".")]
        [Input("m2_input", ".")]
        [Input("l2_input", ".")]
        [Input("w2_input", ".")]
        [Input("p2_input", ".")]
        [Input("u2_input", ".")]
        [Input("t2_input", ".")]
        [Input("q2_input", ".")]
        [Input("n2_input", ".")]
        [Input("s2_input", ".")]
        [Input("v2_input", ".")]
        [Input("c5_2ndlayer", ".")]
        [Input("c6_2ndlayer", ".")]
        [Input("c18_2ndlayer", ".")]
        [Output("Success", "has it written to the file?")]
        public static bool LCAToolPart(string filePath, int c2_input = 2942, int d2_input = 0, int b2_input = 11768, int e2_input = 0, int g2_input = 0, int h2_input = 50,
            int f2_input = 0, int o2_input = 0, int i2_input = 2942, string j2_input = "New Buildings", int m2_input = 1, int l2_input = 50, int w2_input = 0,
            double p2_input = 0.5, double u2_input = 0, double t2_input = 0, double q2_input = 0.1, string n2_input = "No",
            string s2_input = "Gas BHKW (KWKK)", string v2_input = "Second-Life Lithium", double c5_2ndlayer = 188.288, double c6_2ndlayer = 156.9066667,
            string c18_2ndlayer = "Yes")
        {

            double kelvin = 273.15;

            //INPUTS

            int c23_2ndlayer = o2_input;
            int c9_2ndlayer = m2_input;
            int c8_2ndlayer = l2_input;
            int c22_2ndlayer = w2_input;
            double c20_2ndlayer = p2_input;
            double c25_2ndlayer = u2_input;
            double c24_2ndlayer = t2_input;
            double c21_2ndlayer = q2_input;
            string c10_2ndlayer = n2_input;
            string c23_2ndlayer_input = s2_input;
            string c26_2ndlayer = v2_input;


            //Investment [€] - KG200
            //ke - Cost of Emissions table

            //calculate j8_ke Central Heating Infrastructure Costs (KG200 - Central Heating System)

            //for New Buildings
            int e12_assumed = 45;       //e12_assumed - New Building-Office Heat Energy Demand in kwh/m2/a
            int e13_assumed = 45;       //e13_assumed - New Building-Research Heat Energy Demand in kwh/m2/a
            int e14_assumed = 25;       //e14_assumed - New Building-Residential Heat Energy Demand in kwh/m2/a
            int e15_assumed = 45;       //e15_assumed - New Building-Education Heat Energy Demand in kwh/m2/a
            int e16_assumed = 45;       //e16_assumed - New Building-Special Heat Energy Demand in kwh/m2/a
            int e17_assumed = 0;        //e17_assumed - New Building-Parken Heat Energy Demand in kwh/m2/a

            //for Existing Buildings
            int e19_assumed = 80;       //e19_assumed - Existing Buildings-Production Heat Energy Demand in kwh/m2/a
            int e20_assumed = 100;      //e20_assumed - Existing Buildings-Office Heat Energy Demand in kwh/m2/a
            int e21_assumed = 100;      //e21_assumed - Existing Buildings-Research Heat Energy Demand in kwh/m2/a
            int e22_assumed = 90;       //e22_assumed - Existing Buildings-Residential Heat Energy Demand in kwh/m2/a
            int e23_assumed = 90;       //e23_assumed - Existing Buildings-Education Heat Energy Demand in kwh/m2/a
            int e24_assumed = 80;       //e24_assumed - Existing Buildings-Special Heat Energy Demand in kwh/m2/a

            double e5_flachen = 0.8 * c2_input;     //e5_flachen - flachen Office			//INPUT!!!!!!
            double g5_flachen = 0.8 * d2_input;     //g5_flachen - flachen forschung		//INPUT!!!!!!
            double h5_flachen = 0;                  //h5_flachen - flachen temp wohnen	//INPUT!!!!!!
            double i5_flachen = 0.8 * b2_input;     //i5_flachen - flachen wohnen		//INPUT!!!!!!
            double j5_flachen = 0.8 * e2_input;     //j5_flachen - flachen bildung		//INPUT!!!!!!
            double k5_flachen = 0.8 * g2_input;     //k5_flachen - flachen sonder		//INPUT!!!!!!
            double l5_flachen = 0.8 * h2_input;     //l5_flachen - flachen parken		//INPUT!!!!!!
            double d18_flachen = 0.8 * f2_input;    //d18_flachen - flachen production	//INPUT!!!!!!

            // for New Buildings
            int f12_assumed = 5;        //f12_assumed - New Buildings hot water Office Heat Energy Demand in kwh/m2/a
            int f13_assumed = 5;        //f13_assumed - New Buildings hot water forschung Heat Energy Demand in kwh/m2/a
            int f14_assumed = 20;       //f14_assumed - New Buildings hot water wohnen Heat Energy Demand in kwh/m2/a
            int f15_assumed = 10;       //f15_assumed - New Buildings hot water bildung Heat Energy Demand in kwh/m2/a
            int f16_assumed = 5;        //f16_assumed - New Buildings hot water sonder Heat Energy Demand in kwh/m2/a
            int f17_assumed = 0;        //f17_assumed - New Buildings hot water parken Heat Energy Demand in kwh/m2/a

            // for Existing Buildings
            int f19_assumed = 5;        //f19_assumed - Existing Buildings hot water production Heat Energy Demand in kwh/m2/a
            int f20_assumed = 20;       //f20_assumed - Existing Buildings hot water Office Heat Energy Demand in kwh/m2/a
            int f21_assumed = 20;       //f21_assumed - Existing Buildings hot water forschung Heat Energy Demand in kwh/m2/a
            int f22_assumed = 20;       //f22_assumed - Existing Buildings hot water wohnen Heat Energy Demand in kwh/m2/a
            int f23_assumed = 20;       //f23_assumed - Existing Buildings hot water bildung Heat Energy Demand in kwh/m2/a
            int f24_assumed = 20;       //f24_assumed - Existing Buildings hot water sonder Heat Energy Demand in kwh/m2/a


            //Declaring variables

            double n16_heating = 0;
            double n27_heating = 0;
            double n16_hotwater = 0;
            double n27_hotwater = 0;


            if (j2_input == "New Buildings")
            {
                // heating
                double production_heating = 0;
                double office_heating = e12_assumed * e5_flachen;
                double research_heating = e13_assumed * g5_flachen;
                double temp_residential_heating = e14_assumed * h5_flachen;
                double residential_heating = e14_assumed * i5_flachen;
                double education_heating = e15_assumed * j5_flachen;
                double special_heating = e16_assumed * k5_flachen;
                double parken_heating = e17_assumed * l5_flachen;

                n16_heating = production_heating + office_heating + research_heating + temp_residential_heating + residential_heating + education_heating + special_heating + parken_heating;   //n16_heating - total for heating

                // hot water
                double production_hotwater = 0;
                double office_hotwater = f12_assumed * e5_flachen;
                double research_hotwater = f13_assumed * g5_flachen;
                double temp_residential_hotwater = f14_assumed * h5_flachen;
                double residential_hotwater = f14_assumed * i5_flachen;
                double education_hotwater = f15_assumed * j5_flachen;
                double special_hotwater = f16_assumed * k5_flachen;
                double parken_hotwater = 0;

                n16_hotwater = production_hotwater + office_hotwater + research_hotwater + temp_residential_hotwater + residential_hotwater + education_hotwater + special_hotwater + parken_hotwater;  //n16_hotwater - total for hot water

            }
            else if (j2_input == "Existing Buildings")
            {
                // heating

                double production_heating = e19_assumed * d18_flachen;
                double office_heating = e20_assumed * e5_flachen;
                double research_heating = e21_assumed * g5_flachen;
                double temp_residential_heating = e22_assumed * h5_flachen;
                double residential_heating = e22_assumed * i5_flachen;
                double education_heating = e23_assumed * j5_flachen;
                double special_heating = e24_assumed * k5_flachen;
                double parken_heating = 0 * l5_flachen;

                n27_heating = production_heating + office_heating + research_heating + temp_residential_heating + residential_heating + education_heating + special_heating + parken_heating;       //n16_heating - total for heating

                // hot water
                double production_hotwater = f19_assumed * d18_flachen;
                double office_hotwater = f20_assumed * e5_flachen;
                double research_hotwater = f21_assumed * g5_flachen;
                double temp_residential_hotwater = f22_assumed * h5_flachen;
                double residential_hotwater = f22_assumed * i5_flachen;
                double education_hotwater = f23_assumed * j5_flachen;
                double special_hotwater = f24_assumed * k5_flachen;
                double parken_hotwater = 0;

                n27_hotwater = production_hotwater + office_hotwater + research_hotwater + temp_residential_hotwater + residential_hotwater + education_hotwater + special_hotwater + parken_hotwater;  //n16_hotwater - total for hot water
            }

            double aj38_syst = 0.98;                                //aj38_syst - underfloor heating New Buildings (efficiency)
            double am38_syst = n16_heating / 1000.0;
            double ag39_syst = am38_syst / aj38_syst;               //am38_syst - heating (MWh/a), aj38_syst - underfloor heating New Buildings (efficiency)
            double ae38_syst = 0.95;                                //heat exchanger efficiency
            double ab39_syst = ag39_syst / ae38_syst;               //ag39_syst - underfloor heating New Buildings, ae38_syst - heat exchanger (efficiency)

            double am43_syst = n16_hotwater / 1000.0;
            double aj42_syst = 0.95;                                //hot water system New Buildings (efficiency)
            double ag43_syst = am43_syst / aj42_syst;               //am43_syst - hot water (hot water power), aj42_syst - hot water system New Buildings (efficiency)
            double ae45_syst = 0.5;                                 //efficiency
            double ae43_syst = 70.0;                                  //temperature hot water New Buildings
            double ae42_syst = 40.0;                                  //temperature (low-ex network)
            double ae44_syst = (ae43_syst + kelvin) / ((ae43_syst + kelvin) - (ae42_syst + kelvin));     //ae44_syst - heat pump hot water New Buildings COPideal
            double ae46_syst = ae45_syst * ae44_syst;               //ae46_syst - heat pump hot water New Buildings COPreal 
            double ae48_syst = ag43_syst / ae46_syst;               //ag43_syst - hot water system New Buildings (MWh/a)
            double ab43_syst = ag43_syst / ae48_syst;               //ag43_syst - hot water system New Buildings, ae48_syst - heat pump hot water New Buildings (power)

            double am52_syst = n27_heating / 1000.0;
            double aj51_syst = 0.95;                                //convector(heater) Existing Buildings (efficiency)
            double ag52_syst = am52_syst / aj51_syst;               //am52_syst - heating energy, aj51_syst - convector(heater) Existing Buildings (efficiency)

            double ae54_syst = 0.5;                                 //heat pump heizen Existing Buildings (efficiency)
            double ae51_syst = 40.0;                                  //Tc
            double ae52_syst = 50.0;                                  //Th
            double ae53_syst = (ae52_syst + kelvin) / ((ae52_syst + kelvin) - (ae51_syst + kelvin));
            double ae55_syst = ae54_syst * ae53_syst;               //ae55_syst - COPreal
            double ae57_syst = ag52_syst / ae55_syst;               //ag52_syst - convector(heater) Existing Buildings, ae57_syst - heat pump heating Existing Buildings

            double ab52_syst = ag52_syst - ae57_syst;               //ag52_syst - convector(heater) Existing Buildings, ae57_syst - heat pump heating Existing Buildings (power)

            double am61_syst = n27_hotwater / 1000.0;                 //am61_syst - hot water power Existing Buildings
            double aj60_syst = 0.95;                                //hot water system Existing Buildings (efficiency)
            double ag61_syst = am61_syst / aj60_syst;               //ag61_syst - hot water system Existing Buildings, am61_syst - hot water power Existing Buildings, aj60_syst - hot water system Existing Buildings (efficiency)
            double ae63_syst = 0.5;                                 //ae63_syst - heat pump hot water Existing Buildings (efficiency)
            double ae60_syst = 40.0;                                  //Tc
            double ae61_syst = 70.0;                                  //Th
            double ae62_syst = (ae61_syst + kelvin) / ((ae61_syst + kelvin) - (ae60_syst + kelvin));
            double ae64_syst = ae63_syst * ae62_syst;               //ae64_syst - COPreal, ae63_syst - heat pump hot water Existing Buildings (efficiency), ae62_syst - COPideal
            double ae66_syst = ag61_syst / ae64_syst;               //ae64_syst - COPreal
            double ab61_syst = ag61_syst - ae66_syst;               //hot water system Existing Buildings, ae66_syst - heat pump hot water Existing Buildings (power)

            double x43_syst = 0.9;                                  //low-ex network (efficiency)

            double p44_syst = (ab39_syst + ab43_syst + ab52_syst + ab61_syst) / x43_syst;     //ab39_syst - heat exchanger, ab43_syst - heat pump hot water New Buildings, ab52_syst - heat pump heizen Existing Buildings, ab61_syst - heat pump hot water Existing Buildings, x43_syst - efficiency (low-ex netwerk)

            double c7_renewables = o2_input;                        //c7_renewables - available space   INPUT!!!!!
            double c8_renewables = 6.0;                               //radius
            double c9_renewables = (Math.PI * Math.Sqrt(3) / 6 * c7_renewables) / (Math.PI * (Math.Pow(c8_renewables, 2)));   //c9_renewables - maximum value
            double c6_renewables = 1.0;                             //c6_renewables - geo factor
            double c10_renewables = c9_renewables * c6_renewables;  //c10_renewables - anzahl sonden _ number of special??
            double c13_renewables = 100.0;                            //c13_renewables - lange der sonde _ length of the special??
            double c12_renewables = 45.0;                               //c12_renewables - specific heat capacity (W/m)
            double c14_renewables = 1800.0;                           //c14_renewables - load hours (h/a)

            double g71_syst = 0;

            if (p44_syst != 0)
            {
                double c19_renewables = c10_renewables * c13_renewables * c12_renewables * c14_renewables / 1000000;        //c19_renewables - geothermal heat potential
                g71_syst = c19_renewables;                          //(MWh/a)
            }

            double j73_syst = 0.5;                                  //heat pump geothermal (efficiency)
            double j71_syst = 40.0;                                   //heat pump geothermal Th
            double j70_syst = 15.0;                                   //heat pump geothermal Tc                     
            double j72_syst = (j71_syst + kelvin) / ((j71_syst + kelvin) - (j70_syst + kelvin));      //j72_syst - COPideal
            double j74_syst = j73_syst * j72_syst;                  //j74_syst - COPreal

            double l71_syst = g71_syst / (1 - 1 / j74_syst);        //j74_syst - COPreal

            double g61_syst = 0;            //g61_syst - solar energy (MWh/a)
            double j7_renewables = 0;
            double j8_renewables = 0;
            double j9_renewables = 0;
            int j10_renewables = 0;
            double j13_renewables = 0;
            double j14_renewables = 0;
            int p15_flachen = 0;
            int p25_flachen = 0;

            if (p44_syst != 0)
            {
                j13_renewables = 0.65;      //j13_renewables - solar energy efficiency
                j10_renewables = 1046;      //j10_renewables - solar energy global horizontal irradiation (kWh/m2/a)
                p15_flachen = 0;
                p25_flachen = 0;

                if (j2_input == "New Buildings")
                {
                    p15_flachen = i2_input;         //p15_flachen - roof area INPUT!!!!!!
                }
                else if (j2_input == "Existing Buildings")
                {
                    p25_flachen = i2_input;         //p25_flachen - roof area INPUT!!!!!!
                }

                j7_renewables = (p25_flachen + p15_flachen) * q2_input;     //j7_renewables - solar energy flache (m2)
                j8_renewables = 1.0;                //j8_renewables - solar energy utilisation factor
                j9_renewables = j7_renewables * j8_renewables;

                j14_renewables = 0.95;      //j14_renewables - solar energy utilisation factor
                double j19_renewables = j13_renewables * j10_renewables * j9_renewables * j14_renewables / 1000;        //j19_renewables - solar energy heat potential (MWh/a)
                g61_syst = j19_renewables;      //g61_syst - solar energy (MWh/a)
            }


            double j60_syst = 0.95;     //heat exchanger efficiency
            double j62_syst = 1.0;        //heat exchanger utilization    
            double l61_syst = g61_syst * j60_syst * j62_syst;      //l61_syst - heat exchanger (MWh/a)

            //Decalring variables
            double g51_syst = 0;
            double p8_renewables = 0;
            double p10_renewables = 0;
            double p12_renewables = 0;
            double p14_renewables = 0;
            double p7_renewables = 0;
            double p13_renewables = 0;

            if (c18_2ndlayer == "Yes")
            {
                p7_renewables = c5_2ndlayer + c6_2ndlayer;  //p7_renewables - waste water personen	INPUT!!!!!!!!
                p8_renewables = 80.0;           //p8_renewables - abwassermenge (I/d)l51
                p10_renewables = 4.19;      //p10_renewables - waste water specific energiegehalt wasser (kJ/kgWasser/K)
                p12_renewables = 10.0;      //p12_renewables - waste water temperaturedifferenz heat exchanger (K)
                p13_renewables = 1.0 / 3600.0;  //p13_renewables - waste water umrechnung (kWh/kJ)
                p14_renewables = 0.6;       //p14_renewables - waste water efficiency heat exchanger
                double p19_renewables = p7_renewables * p8_renewables * p10_renewables * p12_renewables * p13_renewables * p14_renewables * 365 / 1000;     //p19_renewables - waste water warmepotential
                g51_syst = p19_renewables;      //g51_syst - (MWh/a)
            }

            double j53_syst = 0.5;      //heat pump waste water (efficiency)
            double j51_syst = 40.0;       //heat pump waste water Th
            double j50_syst = 15.0;      //heat pump waste water Tc
            double j52_syst = (j51_syst + kelvin) / ((j51_syst + kelvin) - (j50_syst + kelvin));     //j52_syst - COPideal
            double j54_syst = j53_syst * j52_syst;      //j54_syst - heat pump waste water COPreal
            double l51_syst = g51_syst / (1 - 1.0 / j54_syst);      //l51_syst - heat pump waste water (MWh/a)
            double n47_syst = l71_syst + l61_syst + l51_syst;  //n47_syst - sustainable supply (MWh/a), l71_syst - heat pump geothermal (MWh/a), l61_syst - heat exchanger (MWh/a), l51_syst - heat pump waste water (MWh/a)

            //for New Buildings
            double k12_assumed = 55.0;      //k12_assumed - New Buildings heating office performance in W/m2
            double k13_assumed = 55.0;      //k13_assumed - New Buildings heating research performance in W/m2
            double k14_assumed = 28.0;      //k14_assumed - New Buildings heating residential performance in W/m2
            double k15_assumed = 55.0;      //k15_assumed - New Buildings heating education performance in W/m2
            double k16_assumed = 55.0;      //k16_assumed - New Buildings heating special performance in W/m2
            double k17_assumed = 0.0;           //k17_assumed - New Buildings heating parken performance in W/m2

            double l12_assumed = 15.0;      //l12_assumed - New Buildings cooling office performance in W/m2
            double l13_assumed = 25.0;      //l13_assumed - New Buildings cooling research performance in W/m2
            double l14_assumed = 0.0;           //l14_assumed - New Buildings cooling residential performance in W/m2
            double l15_assumed = 15.0;      //l15_assumed - New Buildings cooling education performance in W/m2
            double l16_assumed = 15.0;      //l16_assumed - New Buildings cooling special performance in W/m2
            double l17_assumed = 0.0;       //l17_assumed - New Buildings cooling parken performance in W/m2

            //for Existing Buildings
            double k19_assumed = 64.0;      //k19_assumed - Existing Buildings heating production performance in W/m2
            double k20_assumed = 80.0;      //k20_assumed - Existing Buildings heating office performance in W/m2
            double k21_assumed = 80.0;      //k21_assumed - Existing Buildings heating research performance in W/m2
            double k22_assumed = 40.0;      //k22_assumed - Existing Buildings heating residential performance in W/m2
            double k23_assumed = 72.0;      //k23_assumed - Existing Buildings heating education performance in W/m2
            double k24_assumed = 80.0;      //k24_assumed - Existing Buildings heating special performance in W/m2

            double l19_assumed = 45.0;      //l19_assumed - Existing Buildings cooling production performance in W/m2
            double l20_assumed = 25.0;      //l20_assumed - Existing Buildings cooling office performance in W/m2
            double l21_assumed = 25.0;      //l21_assumed - Existing Buildings cooling research performance in W/m2
            double l22_assumed = 0.0;           //l22_assumed - Existing Buildings cooling residential performance in W/m2
            double l23_assumed = 20.0;      //l23_assumed - Existing Buildings cooling education performance in W/m2
            double l24_assumed = 20.0;      //l24_assumed - Existing Buildings cooling special performance in W/m2		

            //Declaring variables
            double ac16_heating = 0;
            double ac27_heating = 0;

            if (j2_input == "New Buildings")
            {
                double production_heating_load = 0.0;                                   //s7_heating - production heating load in W
                double office_heating_load = k12_assumed * e5_flachen;              //t7_heating - Office heating load in W
                double research_heating_load = k13_assumed * g5_flachen;            //v7_heating - research heating load in W
                double temp_residential_heating_load = k14_assumed * h5_flachen;    //w7_heating - temp residential heating load in W
                double residential_heating_load = k14_assumed * i5_flachen;         //x7_heating - residential heating load in W
                double education_heating_load = k15_assumed * j5_flachen;           //y7_heating - education heating load in W
                double special_heating_load = k16_assumed * k5_flachen;             //z7_heating - special heating load in W
                double parken_heating_load = 0.0;                                       //aa7_heating - parken heating load in W

                ac16_heating = production_heating_load + office_heating_load + research_heating_load + temp_residential_heating_load + residential_heating_load + education_heating_load + special_heating_load + parken_heating_load;  //ac16_heating - Total heating load in W for New Buildings
                ac27_heating = 0.0;
            }
            else if (j2_input == "Existing Buildings")
            {
                double production_heating_load = k19_assumed * d18_flachen;             //s20_heating - production heating load in W
                double office_heating_load = k20_assumed * e5_flachen;              //t20_heating - office heating load in W
                double research_heating_load = k21_assumed * g5_flachen;            //v20_heating - research heating load in W
                double temp_residential_heating_load = k22_assumed * h5_flachen;    //w20_heating - temp residential heating load in W
                double residential_heating_load = k22_assumed * i5_flachen;         //x20_heating - residential heating load in W
                double education_heating_load = k23_assumed * j5_flachen;           //y20_heating - education heating load in W
                double special_heating_load = k24_assumed * k5_flachen;             //z20_heating - special heating load in W
                double parken_heating_load = 0.0;                                       //aa20_heating - special heating load in W

                ac16_heating = 0.0;
                ac27_heating = production_heating_load + office_heating_load + research_heating_load + temp_residential_heating_load + residential_heating_load + education_heating_load + special_heating_load + parken_heating_load;      //ac16_heating - Total heating load in W for Existing Buildings
            }


            double am36_syst = ac16_heating / 1000.0;         //am36_syst - heating demand max. power
            double ag38_syst = am36_syst / aj38_syst;       //ag38_syst - underfloor heating New Buildings (kW)
            double ab38_syst = ag38_syst / ae38_syst;

            double am42_syst = 0.0;                           //hot water max. power (kW)
            double ag42_syst = am42_syst / aj42_syst;       //ag42_syst - hot water system New Buildings (kW)

            double ae47_syst = ag42_syst / ae46_syst;       //ae47_syst - heat pump hot water New Buildings power output 
            double ab42_syst = ag42_syst - ae47_syst;       //ab42_syst - heat pump hot water New Buildings (kW)

            double am51_syst = ac27_heating / 1000.0;         //am51_syst - Heating demand for  Existing Buildings max. power output (kW)
            double ag51_syst = am51_syst / aj51_syst;       //ag51_syst - convector(heater) Existing Buildings (kW), aj51_syst - convector(heater) Existing Buildings efficiency
            double ab51_syst = ag51_syst - ae51_syst;       //ab51_syst - heat pump heating Existing Buildings (kW)

            double am60_syst = 0.0;                          //hot water max. power
            aj60_syst = 0.95;                               //hot water system Existing Buildings efficiency //Already defined apparently
            double ag60_syst = am60_syst / aj60_syst;       //ag60_syst - hot water system Existing Buildings (kW)

            ae64_syst = ae63_syst / ae62_syst;              //ae64_syst - heat pump hot water Existing Buildings COPreal //Already defined apparently
            double ae65_syst = ag60_syst / ae64_syst;       //ae65_syst - heat pump hot water Existing Buildings power output  (kW)
            double ab60_syst = ag60_syst - ae65_syst;       //ab60_syst - heat pump hot water Existing Buildings (kW)

            double p43_syst = (ab38_syst + ab42_syst + ab51_syst + ab60_syst) / x43_syst;      //p43_syst - production requirements (kW)

            //Declaring variables
            double g70_syst = 0;    //g70_syst - heat pump geothermal (kW)
            double g60_syst = 0;    //g60_syst - solar energy (kW)
            double l70_syst = 0;    //l70_syst - heat pump geothermal (kW)

            if (p44_syst != 0)
            {
                double c18_renewables = c10_renewables * c13_renewables * c12_renewables / 1000.0;  //c18_renewables - power output - geothermal (kW)
                g70_syst = c18_renewables;       //g70_syst - (kW)
                l70_syst = g70_syst / (1 - 1.0 / j74_syst);
                double j11_renewables = 2.866;      //j11_renewables - solar energy (kWh/m2/a)
                double j12_renewables = j11_renewables / 24 * 1000.0;       //j12_renewables - solar energy (W/m2)
                double j18_renewables = j12_renewables * j9_renewables * j13_renewables * j14_renewables / 1000.0;      //j18_renewables - power output solar energy (kW)
                g60_syst = j18_renewables;      //g60_syst - solar energy (kW)
            }
            else
            {
                l70_syst = g70_syst / (1 - 1.0 / j74_syst);
            }

            double j61_syst = 0.7;      //(energy saved or power reduction) winter heat exchanger
            double l60_syst = g60_syst * j60_syst * j61_syst;      //l60_syst - heat exchanger (kW)

            //Declaring variables
            double g50_syst = 0;

            if (c18_2ndlayer == "Yes")   //c18_2ndlayer - INPUT!!!!!!!
            {
                double p9_renewables = p8_renewables / 24.0 / 3600.0;
                double p18_renewables = p7_renewables * p9_renewables * p10_renewables * p12_renewables * p14_renewables * 365.0 / 1000.0;      //p18_renewables - waste water heat output
                g50_syst = p18_renewables;
            }
            else
            {
                g50_syst = 0;
            }

            double l50_syst = g50_syst / (1.0 - 1.0 / j54_syst);        //l50_syst - heat pump waste water (kW)
            double n46_syst = l70_syst + l60_syst + l50_syst;      //sustainable supply

            //Declaring variables

            double d25_syst = 0.9;
            int j30_syst = 6000;     //runtime BHKW (h/a)
            double j28_syst = 0;
            double l34_syst = 0;
            double l40_syst = 0;

            if (p44_syst - n47_syst >= 0)   //p44_syst - matching-generation demand (MWh/a), n47_syst - sustainable supply (MWh/a)
            {
                if (c23_2ndlayer_input == "Gas BHKW (KWKK)") //INPUT!!!!!!
                {
                    double p29_syst = p43_syst - n46_syst;     // p43_syst - production demand(kW), n46_syst - sustainable supply(kW)
                    double p30_syst = p44_syst - n47_syst;     //p30_syst - production demand (kW)
                    j28_syst = p30_syst * 1000.0 * d25_syst / j30_syst;      //j28_syst - BHKW heat power
                    l34_syst = p29_syst - j28_syst;  //p29_syst - production demand (kW), j29_syst - BHKW heat power
                }

                if (c23_2ndlayer_input == "AirHeatPump") //INPUT!!!!!!
                {
                    l40_syst = p43_syst - n46_syst;

                }

            }

            double g8_ke = l34_syst + l40_syst;     //l34_syst - peak load boilers, l40_syst - generation demand for air heat pump
            int d8_ke = 110;
            double j8_ke = d8_ke * g8_ke;     //j8_ke - central heating costs, d8_ke - specific, g8_ke - quantity
            Console.WriteLine("j8_ke" + "   " + j8_ke);

            //calculate j9_ke - infrastructure EMSR costs (kg200 - central heating)
            int d9_ke = 55;      //d9_ke - specific
            double g9_ke = p43_syst;       //g9_ke - quantity
            double j9_ke = d9_ke * g9_ke;    //j9_ke - EMSR costs, d9_ke - specific, g9_ke - quantity

            Console.WriteLine("j9_ke" + "   " + j9_ke);

            //calculate j10_ke - infrastructure storage tank costs (kg200 - central heating)
            int d10_ke = 6640;       //d10_ke - specific
            double g10_ke = c9_2ndlayer;        //INPUT!!!!!!
            double j10_ke = d10_ke * g10_ke;     //j10_ke - storage tank costs, d10_ke - specific, g10_ke - quantity
            Console.WriteLine("j10_ke" + "   " + j10_ke);

            //calculate j11_ke - infrastructure mains connection costs (kg200 - central heating)
            int d11_ke = 3500;      //d11_ke - specific
            double g11_ke = c9_2ndlayer;    //INPUT!!!!!
            double j11_ke = d11_ke * g11_ke;     //j11_ke - mains connection costs, d11_ke - specific, g11_ke - quantity
            Console.WriteLine("j11_ke" + "   " + j11_ke);

            //calculate j13_ke - infrastructure line DN100 costs (kg200 - central heating)
            int d13_ke = 215;        //d13_ke - specific
            double g13_ke = c8_2ndlayer * 0.09;   //INPUT!!!!!!
            double j13_ke = d13_ke * g13_ke;     //j13_ke - low-ex network line DN100 costs, d13_ke - specific, g13_ke - quantity
            Console.WriteLine("j13_ke" + "   " + j13_ke);

            //calculate j14_ke - infrastructure line DN150 costs (kg200 - central heating)
            int d14_ke = 286;        //d14_ke - specific
            double g14_ke = c8_2ndlayer * 0.5;  //INPUT!!!!!!
            double j14_ke = d14_ke * g14_ke;     //j14_ke - low-ex network line DN150 costs, d14_ke - specific, g14_ke - quantity
            Console.WriteLine("j14_ke" + "   " + j14_ke);

            //calculate j15_ke - infrastructure line DN300 costs (kg200 - central heating)
            double d15_ke = 550;       //d15_ke - specific
            double g15_ke = c8_2ndlayer * 0.41;   //INPUT!!!!!!
            double j15_ke = d15_ke * g15_ke;     //j15_ke - low-ex network line DN300 costs, d15_ke - specific, g15_ke - quantity
            Console.WriteLine("j15_ke" + "   " + j15_ke);

            //calculate j16_ke - infrastructure low-ex network kernbohrungen costs (kg200 - central heating)
            double d16_ke = 500;        //d16_ke - specific
            double g16_ke = c9_2ndlayer;   //INPUT!!!!!!
            double j16_ke = d16_ke * g16_ke;     //j16_ke - low-ex network kernbohrungen costs, d16_ke - specific, g16_ke - quantity
            Console.WriteLine("j16_ke" + "   " + j16_ke);

            //calculate j17_ke - infrastructure low-ex network demand for local heating network according to § 18 KWKG costs (kg200 - central heating)
            double d17_ke = 0.3;       //d17_ke - specific
            double j17_ke = (j13_ke + j14_ke + j15_ke + j16_ke) * d17_ke * (-1);    //j17_ke - low-ex network demand for local heating network according to § 18 KWKG costs, d17_ke - specific, g17_ke - quantity
            Console.WriteLine("j17_ke" + "   " + j17_ke);

            //calculate j20_ke - energy source BHKW costs (kg200 - central heating)
            double d20_ke = 950;      //d20_ke - specific
            double g20_ke = j28_syst;   //g20_ke - quantity
            double j20_ke = d20_ke * g20_ke;     //j20_ke - energy source BHKW costs, d20_ke - specific, g20_ke - quantity
            Console.WriteLine("j20_ke" + "   " + j20_ke);

            //calculate j22_ke - energy source waste water costs (kg200 - central heating)
            double d22_ke = 800;        //d22_ke - specific
            double g22_ke = l50_syst;   //g22_ke - quantity
            double j22_ke = d22_ke * g22_ke;     //j22_ke - energy source waste water costs, d22_ke - specific, g22_ke - quantity
            Console.WriteLine("j22_ke" + "   " + j22_ke);

            //calculate j24_ke - energy source demand costs (kg200 - central heating)
            double d24_ke = 0.3;        //d24_ke - specific
            double j24_ke = (-1) * d24_ke * j22_ke;     //j24_ke - energy source demand costs, d24_ke - specific, j22_ke - energy source waste water costs
            Console.WriteLine("j24_ke" + "   " + j24_ke);

            //calculate j27_ke - energy source solar energy costs (kg200 - central heating)
            double d27_ke = 300;        //d27_ke - specific
            double g27_ke = j7_renewables;   //g27_ke - quantity
            double j27_ke = d27_ke * g27_ke;     //j27_ke - energy source solar energy costs, d27_ke - specific, g27_ke - quantity
            Console.WriteLine("j27_ke" + "   " + j27_ke);

            //calculate j30_ke - energy source geothermal costs (kg200 - central heating)
            double d30_ke = 2400;        //d30_ke - specific
            double g30_ke = g70_syst;   //g30_ke - quantity
            double j30_ke = d30_ke * g30_ke;     //j30_ke - energy source geothermal costs, d30_ke - specific, g30_ke - quantity
            Console.WriteLine("j30_ke" + "   " + j30_ke);

            //calculate j31_ke - energy source heat pump central costs (kg200 - central heating)
            double d31_ke = 8000 / 20;        //d31_ke - specific
            double g31_ke = l50_syst + l60_syst + l70_syst + l40_syst;   //g31_ke - quantity
            double j31_ke = d31_ke * g31_ke;     //j31_ke - energy source heat pump central costs, d31_ke - specific, g31_ke - quantity
            Console.WriteLine("j31_ke" + "   " + j31_ke);

            double sum_inv_kg200 = j8_ke + j9_ke + j10_ke + j11_ke + j13_ke + j14_ke + j15_ke + j16_ke + j17_ke + j20_ke + j22_ke + j24_ke + j27_ke + j30_ke + j31_ke;

            Console.WriteLine("sum_inv_kg200 = " + "   " + sum_inv_kg200);

            //Investment Costs [€] - KG400-Technology

            //calculate j34_ke - increasing heat pump decentralise costs (kg400 - decentralised facilities)
            double d34_ke = 6000.0 / 20.0;    //d34_ke - specific
            double g34_ke = (ae48_syst + ae57_syst + ae66_syst) / 5000.0 * 1000.0;
            double j34_ke = d34_ke * g34_ke;     //j34_ke - increasing heat pump decentralise costs, d34_ke - specific, g34_ke - quantity

            //calculate j35_ke - building TGA pro m2 BGF costs (kg400 - decentralised facilities)
            double d35_ke = 264.0;   //d35_ke - specific
            double d5_flachen = f2_input * 0.8;     //d5_flachen - production NUF New Buildings INPUT!!!!!!!!

            //Declaring variables
            double n15_flachen = 0;
            double n25_flachen = 0;

            if (j2_input == "New Buildings")
            {
                n15_flachen = d5_flachen + e5_flachen + g5_flachen + h5_flachen + i5_flachen + j5_flachen + k5_flachen + l5_flachen;
            }
            else
            {
                n25_flachen = d5_flachen + e5_flachen + g5_flachen + h5_flachen + i5_flachen + j5_flachen + k5_flachen + l5_flachen;
            }

            double g35_ke = n15_flachen;    //g35_ke - quantity
            double j35_ke = d35_ke * g35_ke;     //j35_ke - building TGA pro m2 BGF costs, d35_ke - specific, g35_ke - quantity

            //calculate j38_ke - installation costs (electricity grid)
            double d38_ke = 120000.0;    //d38_ke - specific

            //Declaring variables
            double g38_ke = 0;

            if (c10_2ndlayer == "Yes")      //INPUT!!!!!!!!! 
            {
                g38_ke = 1;  //g38_ke - quantity
            }

            double j38_ke = d38_ke * g38_ke;     //j38_ke - installation costs, d38_ke - specific, g38_ke - quantity

            double sum_inv_kg400 = j34_ke + j35_ke + j38_ke;
            Console.WriteLine("sum_inv_kg400 = " + "   " + sum_inv_kg400);

            //Investment Costs [€] - wind_power

            //calculate j44_ke - wind_power offshore costs (wind_power)
            double d44_ke = 3500.0;      //d44_ke - specific
            double g44_ke = c22_2ndlayer * 1000.0;     //INPUT!!!!!
            double inv_wind_power = d44_ke * g44_ke;  //j44_ke - wind_power offshore costs, d44_ke - specific, g44_ke - quantity
            Console.WriteLine("inv_wind_power = " + "   " + inv_wind_power);


            //Investment Costs [€] - PV

            //calculate j48_ke - PV total costs (PV)
            double d48_ke = 1200.0;       //d48_ke - specific

            //Declaring variables
            double q15_flachen = 0;
            double q25_flachen = 0;

            if (j2_input == "New Buildings")
            {
                q15_flachen = i2_input * c20_2ndlayer;      //INPUT!!!!!!
            }
            else
            {
                q25_flachen = i2_input * c20_2ndlayer;      //INPUT!!!!!!
            }

            double c8_solar = q25_flachen + q15_flachen;    //c8_solar - total effective area solar (m2)
            double c5_solar = 0.16;     //c5_solar - PV efficiency solar
            double c9_solar = c8_solar * c5_solar;      //c9_solar - leistung solar (kW)
            double g48_ke = c9_solar;      //g48_ke - mercedes PV preis (kWp)
            double inv_pv = d48_ke * g48_ke;   //j48_ke - PV total costs, d48_ke - specific, g48_ke - quantity
            Console.WriteLine("inv_pv = " + "   " + inv_pv);

            //Investment Costs [€] - Battery

            //calculate j51_ke - battery tesia powerpacks costs
            double d51_ke = 55000.0;       //d51_ke - specific
            double j4_solar = c25_2ndlayer;     //INPUT!!!!!!
            double g51_ke = Math.Round(j4_solar / 210.0 / 0.3) * 0.3;    //g51_ke - powerpacks
            double inv_battery = d51_ke * g51_ke;   //j51_ke - battery costs, d51_ke - specific, g51_ke - quantity
            Console.WriteLine("inv_battery = " + "   " + inv_battery);


            //Investment Costs [€] - Local Heat Storage

            //calculate j55_ke - local_heat_storage costs
            double d55_ke = 16000 / 10;      //d55_ke - specific
            double n4_local_heat_storage = c24_2ndlayer;  //INPUT!!!!!
            double radius_tank = 1.5;      //n5_local_heat_storage
            double hohe = 7;   //n6_local_heat_storage
            double n7_local_heat_storage = hohe * Math.PI * Math.Pow(radius_tank, 2);  //  was originally n7_local_heat_storage = hohe* pi*radius_tank**2
            double g55_ke = n4_local_heat_storage * n7_local_heat_storage;     //g55_ke - (m3)
            double inv_local_heat_storage = d55_ke * g55_ke;   //j55_ke - local_heat_storage costs, d55_ke - specific, g55_ke - quantity
            Console.WriteLine("inv_local_heat_storage =" + "   " + inv_local_heat_storage);


            //Investment Costs [€] - Total

            //calculate Investment Costs total
            double inv_total = sum_inv_kg200 + sum_inv_kg400 + inv_wind_power + inv_pv + inv_battery + inv_local_heat_storage;
            Console.WriteLine("inv_total = " + "   " + inv_total);

            //Embodied THG Emission [tCO2e] - KG200

            //calculate s8_ke - infrastructure central heating embodied emissions (kg200 - central heating)

            double p8_ke = 1.53;    //p8_ke - specific embodied emissions
            double n8_ke = n15_flachen + n25_flachen;   //n8_ke - quantity
            double s8_ke = p8_ke * n8_ke / 1000.0;    //s8_ke - infrastructure central heating embodied emissions

            //calculate s9_ke - infrastructure EMSR embodied emissions (kg200 - central heating)

            double p9_ke = 12.7;    //p9_ke - specific embodied emissions
            double n9_ke = n8_ke;   //n9_ke - quantity
            double s9_ke = p9_ke * n9_ke / 1000.0;   //s9_ke - infrastructure EMSR embodied emissions

            //calculate s10_ke - infrastructure storage tank embodied emissions (kg200 - central heating)

            double p10_ke = 3.5;    //p10_ke - specific embodied emissions
            double n10_ke = g10_ke * 2000.0;   //n10_ke - quantity
            double s10_ke = p10_ke * n10_ke / 1000.0;   //s10_ke - infrastructure storage tank embodied emissions

            //calculate s12_ke - infrastructure low-ex netwerk embodied emissions (kg200 - central heating)

            double p12_ke = 1260.0;    //p12_ke - specific embodied emissions
            double g12_ke = c8_2ndlayer;   //INPUT!!!!!!
            double n12_ke = g12_ke;   //n12_ke - quantity
            double s12_ke = p12_ke * n12_ke / 1000.0;    //s12_ke - infrastructure low-ex netwerk embodied emissions

            //calculate s20_ke - energy source BHKW embodied emissions (kg200 - central heating)

            double p20_ke = 2180.0 / 8.0 / 2.0;    //p20_ke - specific embodied emissions
            double n20_ke = g20_ke;   //n20_ke - quantity
            double s20_ke = p20_ke * n20_ke / 1000.0;   //s20_ke - energy source BHKW embodied emissions

            //calculate s22_ke - energy source waste water embodied emissions (kg220 - central heating)

            double p22_ke = 2180.0 / 8.0;    //p22_ke - specific embodied emissions
            double n22_ke = g22_ke;   //n22_ke - quantity
            double s22_ke = p22_ke * n22_ke / 1000.0;   //s22_ke - energy source waste water embodied emissions


            //calculate s27_ke - energy source solar energy embodied emissions (kg270 - central heating)

            double p27_ke = 155.0;    //p27_ke - specific embodied emissions
            j7_renewables = (p25_flachen + p15_flachen) * c21_2ndlayer;   //INPUT!!!!!
            j8_renewables = 1.0;      //utilisation factor solar energy
            double n27_ke = j7_renewables * j8_renewables;   //n27_ke - quantity
            double s27_ke = p27_ke * n27_ke / 1000.0;    //s27_ke - energy source solar energy embodied emissions

            //calculate s30_ke - energy source geothermal embodied emissions (kg300 - central heating)

            double p30_ke = 28.1;    //p30_ke - specific embodied emissions

            //Declaring variables
            double n30_ke = 0;
            if (n8_ke != 0)
            {
                n30_ke = c10_renewables * c13_renewables;      //n30_ke - quantity
            }

            double s30_ke = p30_ke * n30_ke / 1000.0;    //s30_ke - energy source geothermal embodied emissions

            //calculate s31_ke - energy source heat pump central embodied emissions

            double p31_ke = 2180.0 / 8.0;       //p31_ke - specific embodied emissions
            double n31_ke = l50_syst + l60_syst + l70_syst + l40_syst;          //n31_ke - quantity
            double s31_ke = p31_ke * n31_ke / 1000.0;    //s31_ke - energy source heat pump central embodied emissions

            double embodied_thg_em_kg200 = s8_ke + s9_ke + s10_ke + s12_ke + s20_ke + s22_ke + s27_ke + s30_ke + s31_ke;        //embodied_thg_em_kg200 - Embodied THG Emissions [tCO2e] - KG200
            Console.WriteLine("embodied_thg_em_kg200 = " + "   " + embodied_thg_em_kg200);

            //Embodied THG Emissions [tCO2e] - KG400-Technology

            //calculate s34_ke - increasing heat pump decentral embodied emissions

            double p34_ke = 2910.0 / 8.0;       //p34_ke - specific embodied emissions
            double n34_ke = (ae48_syst + ae57_syst + ae66_syst) / 5000.0 * 1000.0;      //n34_ke - quantity
            double s34_ke = p34_ke * n34_ke / 1000.0;           //s34_ke - increasing heat pump decentral

            //calculate s35_ke - building embodied emissions

            double p35_ke = 3.07 + 6.06;    //p35_ke - specific embodied emissions
            double n35_ke = n15_flachen + n25_flachen;      //n35_ke - quantity		INPUT!!!!!
            double s35_ke = p35_ke * n35_ke / 1000.0;       //s35_ke - building embodied emissions

            double embodied_thg_em_kg400 = s34_ke + s35_ke;     //embodied_thg_em_kg400 - Embodied THG Emissions [tCO2e] - KG400-Technology
            Console.WriteLine("embodied_thg_em_kg400 = " + "   " + embodied_thg_em_kg400);


            // Embodied THG Emissions [tCO2e] - wind_power

            double p44_ke = 1600.0 / 3.0;       //p44_ke - specific
            double n44_ke = c22_2ndlayer;       //n44_ke - quantity c22_2ndlayer - INPUT!!!!!!
            double s44_ke = p44_ke * n44_ke;        //s44_ke - embodied emissions - wind_power
            double embodied_thg_em_wind = s44_ke;
            Console.WriteLine("embodied_thg_em_wind = " + "   " + embodied_thg_em_wind);

            //Embodied THG Emissions [tCO2e] - PV

            double p48_ke = 2080.0;     //p48_ke - specific
            double n48_ke = c9_solar;       //n48_ke - quantity
            double s48_ke = p48_ke * n48_ke / 1000.0;   //s48_ke - embodied emissions - PV total
            double embodied_thg_em_pv = s48_ke;
            Console.WriteLine("embodied_thg_em_pv = " + "   " + embodied_thg_em_pv);

            //Embodied THG Emissions [tCO2e] - Battery

            double p52_ke = 185.0;      //p52_ke - specific
            double n52_ke = c25_2ndlayer;   //INPUT!!!!
            double s52_ke = p52_ke * n52_ke / 1000.0;       //s52_ke - embodied emissions battery alternative calculation
            double p51_ke = 80.6;       //p51_ke - specific
            double n51_ke = c25_2ndlayer;       //INPUT!!!!!
            double s51_ke = p51_ke * n51_ke / 1000.0;

            //Declaring variables
            double embodied_thg_em_bat = 0;

            if (c26_2ndlayer == "Lithium") //INPUT!!!!!
            {
                embodied_thg_em_bat = s52_ke;       //s52_ke - embodied emissions battery alternative calculation//INPUT!!!!!
            }
            else if (c26_2ndlayer == "Second-Life Lithium")
            {
                embodied_thg_em_bat = s51_ke;   //s51_ke - embodied emissions battery tesia powerpacks
            }

            //Embodied THG Emissions [tCO2e] - Local Heat Storage

            double p55_ke = 540.0 / 4.0 * 3.51;     //p55_ke - specific, 3.51 - stahlblech verzinkt
            double n55_ke = n4_local_heat_storage * n7_local_heat_storage;      //n55_ke - quantity
            double embodied_thg_em_nahw = p55_ke * n55_ke / 1000.0;

            Console.WriteLine("embodied_thg_em_nahw = " + "   " + embodied_thg_em_nahw);

            //Embodied THG Emissions [tCO2e] - Total

            //embodied_thg_em_total = embodied_thg_em_kg200 + embodied_thg_em_kg400 + embodied_thg_em_kg300 + embodied_thg_em_wind + embodied_thg_em_pv + embodied_thg_em_bat + embodied_thg_em_nahw

            double embodied_thg_em_total = embodied_thg_em_kg200 + embodied_thg_em_kg400 + embodied_thg_em_wind + embodied_thg_em_pv + embodied_thg_em_bat + embodied_thg_em_nahw;
            Console.WriteLine("embodied_thg_em_total = " + "   " + embodied_thg_em_total);

            //create result csv
            List<string> fieldnames = new List<string>
            {
                "id",
                "Investment Costs - KG200",
                "Investment Costs - KG400-Technology",
                "Investment Costs - wind_power",
                "Investment Costs - PV",
                "Investment Costs - Battery",
                "Investment Costs - Local Heat Storage",
                "Investment Costs - Total",
                "Embodied THG Emissions - KG200",
                "Embodied emissions - KG400-Technology",
                "Embodied THG Emissions - wind_power",
                "Embodied THG Emissions - PV",
                "Embodied THG Emissions - Battery",
                "Embodied THG Emissions - Local Heat Storage",
                "Embodied THG Emissions - Total"
            };

            List<string> data = new List<string>
            {
                "20430",
                (Math.Round(sum_inv_kg200,3)).ToString(),
                (Math.Round(sum_inv_kg400,3)).ToString(),
                (Math.Round(inv_wind_power,3)).ToString(),
                (Math.Round(inv_pv,3)).ToString(),
                (Math.Round(inv_battery,3)).ToString(),
                (Math.Round(inv_local_heat_storage,3)).ToString(),
                (Math.Round(inv_total,3)).ToString(),
                (Math.Round(embodied_thg_em_kg200,3)).ToString(),
                (Math.Round(embodied_thg_em_kg400,3)).ToString(),
                (Math.Round(embodied_thg_em_wind,3)).ToString(),
                (Math.Round(embodied_thg_em_pv,3)).ToString(),
                (Math.Round(embodied_thg_em_bat,3)).ToString(),
                (Math.Round(embodied_thg_em_nahw,3)).ToString(),
                (Math.Round(embodied_thg_em_total,3)).ToString()
            };

            //var export_file = @"C:\Users\eadebowale\Desktop\Testing\LCA_tool_result.csv";
            //        export_file = open("C:/Users/eusmanova/Downloads/LCA_tool_result.csv", "w")

            String separator = ",";
            StringBuilder output = new StringBuilder();

            output.AppendLine(string.Join(separator, fieldnames));
            output.AppendLine(string.Join(separator, data));

            try
            {
                File.WriteAllText(filePath, output.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Data could not be written to the CSV file.");
                return false;
            }

        }
    }
}


