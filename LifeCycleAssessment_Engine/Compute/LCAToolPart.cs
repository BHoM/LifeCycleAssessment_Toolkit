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

            //calculate j8_ke Central Heating Infrastructure Costs (KG200 - Central Heating System) Heat Energy Demand in kwh/m2/a

            //New Buildings - Heat Energy Demand in kwh/m2/a
            int e12_assumed = 45;       //Office
            int e13_assumed = 45;       //Research
            int e14_assumed = 25;       //Residenital
            int e15_assumed = 45;       //Education
            int e16_assumed = 45;       //Special
            int e17_assumed = 0;        //Car park

            //Existing Buildings - Heat Energy Demand in kwh/m2/a
            int e19_assumed = 80;       //Production
            int e20_assumed = 100;      //Office
            int e21_assumed = 100;      //Research
            int e22_assumed = 90;       //Residential
            int e23_assumed = 90;       //Education
            int e24_assumed = 80;       //Special

            //Area
            double e5_area = 0.8 * c2_input;     //Office area
            double g5_area = 0.8 * d2_input;     //Research area		
            double h5_area = 0;                  //Temp residential area	
            double i5_area = 0.8 * b2_input;     //Residential area		
            double j5_area = 0.8 * e2_input;     //Education area		
            double k5_area = 0.8 * g2_input;     //Special area	
            double l5_area = 0.8 * h2_input;     //Car park area	
            double d18_area = 0.8 * f2_input;    //Production area

            //New Buildings - Hot Water Heat Energy Demand in kwh/m2/a 
            int f12_assumed = 5;        //Office
            int f13_assumed = 5;        //Research
            int f14_assumed = 20;       //Residential
            int f15_assumed = 10;       //Education
            int f16_assumed = 5;        //Special
            int f17_assumed = 0;        //Car park

            //Existing Buildings - Hot Water Heat Energy Demand in kwh/m2/a 
            int f19_assumed = 5;        //production
            int f20_assumed = 20;       //Office
            int f21_assumed = 20;       //research
            int f22_assumed = 20;       //residential
            int f23_assumed = 20;       //education
            int f24_assumed = 20;       //sonder

            //Declaring variables
            double n16_heating = 0;
            double n27_heating = 0;
            double n16_hotwater = 0;
            double n27_hotwater = 0;


            if (j2_input == "New Buildings")
            {
                // heating
                double production_heating = 0;
                double office_heating = e12_assumed * e5_area;
                double research_heating = e13_assumed * g5_area;
                double temp_residential_heating = e14_assumed * h5_area;
                double residential_heating = e14_assumed * i5_area;
                double education_heating = e15_assumed * j5_area;
                double special_heating = e16_assumed * k5_area;
                double carpark_heating = e17_assumed * l5_area;

                n16_heating = production_heating + office_heating + research_heating + temp_residential_heating + residential_heating + education_heating + special_heating + carpark_heating;   //n16_heating - total for heating

                // hot water
                double production_hotwater = 0;
                double office_hotwater = f12_assumed * e5_area;
                double research_hotwater = f13_assumed * g5_area;
                double temp_residential_hotwater = f14_assumed * h5_area;
                double residential_hotwater = f14_assumed * i5_area;
                double education_hotwater = f15_assumed * j5_area;
                double special_hotwater = f16_assumed * k5_area;
                double carpark_hotwater = 0;

                n16_hotwater = production_hotwater + office_hotwater + research_hotwater + temp_residential_hotwater + residential_hotwater + education_hotwater + special_hotwater + carpark_hotwater;  //n16_hotwater - total for hot water

            }
            else if (j2_input == "Existing Buildings")
            {
                // heating

                double production_heating = e19_assumed * d18_area;
                double office_heating = e20_assumed * e5_area;
                double research_heating = e21_assumed * g5_area;
                double temp_residential_heating = e22_assumed * h5_area;
                double residential_heating = e22_assumed * i5_area;
                double education_heating = e23_assumed * j5_area;
                double special_heating = e24_assumed * k5_area;
                double carpark_heating = 0 * l5_area;

                n27_heating = production_heating + office_heating + research_heating + temp_residential_heating + residential_heating + education_heating + special_heating + carpark_heating;       //n27_heating - total for heating

                // hot water
                double production_hotwater = f19_assumed * d18_area;
                double office_hotwater = f20_assumed * e5_area;
                double research_hotwater = f21_assumed * g5_area;
                double temp_residential_hotwater = f22_assumed * h5_area;
                double residential_hotwater = f22_assumed * i5_area;
                double education_hotwater = f23_assumed * j5_area;
                double special_hotwater = f24_assumed * k5_area;
                double carpark_hotwater = 0;

                n27_hotwater = production_hotwater + office_hotwater + research_hotwater + temp_residential_hotwater + residential_hotwater + education_hotwater + special_hotwater + carpark_hotwater;  //n27_hotwater - total for hot water
            }

            double aj38_syst = 0.98;                                //Underfloor heating New Buildings (efficiency)
            double am38_syst = n16_heating / 1000.0;                //Heating (MWh/a)
            double ag39_syst = am38_syst / aj38_syst;               //underfloor heating New Buildings
            double ae38_syst = 0.95;                                //heat exchanger efficiency
            double ab39_syst = ag39_syst / ae38_syst;               //heat exchanger demand

            double am43_syst = n16_hotwater / 1000.0;               //hot water (hot water power)
            double aj42_syst = 0.95;                                //hot water system New Buildings (efficiency)
            double ag43_syst = am43_syst / aj42_syst;               //hot water system New Buildings (MWh/a)
            double ae45_syst = 0.5;                                 //efficiency
            double ae43_syst = 70.0;                                //temperature hot water New Buildings
            double ae42_syst = 40.0;                                //temperature (low-ex network)
            double ae44_syst = (ae43_syst + kelvin) / ((ae43_syst + kelvin) - (ae42_syst + kelvin));     //heat pump hot water New Buildings COPideal
            double ae46_syst = ae45_syst * ae44_syst;               //heat pump hot water New Buildings COPreal 
            double ae48_syst = ag43_syst / ae46_syst;               //heat pump hot water New Buildings (power)
            double ab43_syst = ag43_syst / ae48_syst;               //heat pump hot water New Buildings (?demand)

            double am52_syst = n27_heating / 1000.0;                //heating energy
            double aj51_syst = 0.95;                                //convector(heater) Existing Buildings (efficiency)
            double ag52_syst = am52_syst / aj51_syst;               //convector(heater) Existing Buildings

            double ae54_syst = 0.5;                                 //heat pump heizen Existing Buildings (efficiency)
            double ae51_syst = 40.0;                                  //Tc
            double ae52_syst = 50.0;                                  //Th
            double ae53_syst = (ae52_syst + kelvin) / ((ae52_syst + kelvin) - (ae51_syst + kelvin));
            double ae55_syst = ae54_syst * ae53_syst;               //COPreal
            double ae57_syst = ag52_syst / ae55_syst;               //heat pump heating Existing Buildings (power)

            double ab52_syst = ag52_syst - ae57_syst;               //heat pump heating Existing Buildings 

            double am61_syst = n27_hotwater / 1000.0;               //hot water power Existing Buildings
            double aj60_syst = 0.95;                                //hot water system Existing Buildings (efficiency)
            double ag61_syst = am61_syst / aj60_syst;               //hot water system Existing Buildings
            double ae63_syst = 0.5;                                 //heat pump hot water Existing Buildings (efficiency)
            double ae60_syst = 40.0;                                  //Tc
            double ae61_syst = 70.0;                                  //Th
            double ae62_syst = (ae61_syst + kelvin) / ((ae61_syst + kelvin) - (ae60_syst + kelvin)); //COPideal
            double ae64_syst = ae63_syst * ae62_syst;               //heat pump hot water Existing Buildings COPreal
            double ae66_syst = ag61_syst / ae64_syst;               //heat pump hot water Existing Buildings (power)
            double ab61_syst = ag61_syst - ae66_syst;               //hot water system Existing Buildings

            double x43_syst = 0.9;                                  //low-ex network (efficiency)

            double p44_syst = (ab39_syst + ab43_syst + ab52_syst + ab61_syst) / x43_syst;  //matching - generation demand(MWh / a)

            //Renewables
            double c7_renewables = o2_input;                        //available space 
            double c8_renewables = 6.0;                             //radius
            double c9_renewables = (Math.PI * Math.Sqrt(3) / 6 * c7_renewables) / (Math.PI * (Math.Pow(c8_renewables, 2)));   //maximum value
            double c6_renewables = 1.0;                             //geo factor
            double c10_renewables = c9_renewables * c6_renewables;  //Number of samples
            double c13_renewables = 100.0;                          //Sample length
            double c12_renewables = 45.0;                           //specific heat capacity (W/m)
            double c14_renewables = 1800.0;                         //load hours (h/a)

            double g71_syst = 0;

            if (p44_syst != 0)
            {
                double c19_renewables = c10_renewables * c13_renewables * c12_renewables * c14_renewables / 1000000;        //geothermal heat potential
                g71_syst = c19_renewables;                          //(MWh/a)
            }

            double j73_syst = 0.5;                                  //heat pump geothermal (efficiency)
            double j71_syst = 40.0;                                   //heat pump geothermal Th
            double j70_syst = 15.0;                                   //heat pump geothermal Tc                     
            double j72_syst = (j71_syst + kelvin) / ((j71_syst + kelvin) - (j70_syst + kelvin));      //COPideal
            double j74_syst = j73_syst * j72_syst;                  //COPreal

            double l71_syst = g71_syst / (1 - 1 / j74_syst);        // heat pump geothermal (MWh/a)


            double g61_syst = 0;            
            double j7_renewables = 0;
            double j8_renewables = 0;
            double j9_renewables = 0;
            int j10_renewables = 0;
            double j13_renewables = 0;
            double j14_renewables = 0;
            int p15_area = 0;
            int p25_area = 0;

            if (p44_syst != 0)
            {
                j13_renewables = 0.65;      //solar energy efficiency
                j10_renewables = 1046;      //solar energy global horizontal irradiation (kWh/m2/a)
                p15_area = 0;
                p25_area = 0;

                if (j2_input == "New Buildings")
                {
                    p15_area = i2_input;         //roof area 
                }
                else if (j2_input == "Existing Buildings")
                {
                    p25_area = i2_input;         //roof area
                }

                j7_renewables = (p25_area + p15_area) * q2_input;     //solar energy area (m2)
                j8_renewables = 1.0;                //solar energy utilisation factor
                j9_renewables = j7_renewables * j8_renewables;

                j14_renewables = 0.95;      //solar energy utilisation factor
                double j19_renewables = j13_renewables * j10_renewables * j9_renewables * j14_renewables / 1000;        //solar energy heat potential (MWh/a)
                g61_syst = j19_renewables;      //solar energy (MWh/a)
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
                p7_renewables = c5_2ndlayer + c6_2ndlayer;  // waste water personen
                p8_renewables = 80.0;                       //amount of waste water (I/d)l51
                p10_renewables = 4.19;                      //waste water specific energiegehalt wasser (kJ/kgWasser/K)
                p12_renewables = 10.0;                      //waste water temperaturedifferenz heat exchanger (K)
                p13_renewables = 1.0 / 3600.0;              //waste water conversion (kWh/kJ)
                p14_renewables = 0.6;                       //waste water efficiency heat exchanger
                double p19_renewables = p7_renewables * p8_renewables * p10_renewables * p12_renewables * p13_renewables * p14_renewables * 365 / 1000;     //waste water warmepotential
                g51_syst = p19_renewables;      //(MWh/a)
            }

            double j53_syst = 0.5;      //heat pump waste water (efficiency)
            double j51_syst = 40.0;       //heat pump waste water Th
            double j50_syst = 15.0;      //heat pump waste water Tc
            double j52_syst = (j51_syst + kelvin) / ((j51_syst + kelvin) - (j50_syst + kelvin));     //COPideal
            double j54_syst = j53_syst * j52_syst;      //heat pump waste water COPreal
            double l51_syst = g51_syst / (1 - 1.0 / j54_syst);      //heat pump waste water (MWh/a)
            double n47_syst = l71_syst + l61_syst + l51_syst;  //sustainable supply (MWh/a)


            //New Buildings - heating performance in W/m2
            double k12_assumed = 55.0;      //office
            double k13_assumed = 55.0;      //research
            double k14_assumed = 28.0;      //residential
            double k15_assumed = 55.0;      //education
            double k16_assumed = 55.0;      //special
            double k17_assumed = 0.0;       //carpark

            //New Buildings - cooling performance in W/m2
            double l12_assumed = 15.0;      //office
            double l13_assumed = 25.0;      //research
            double l14_assumed = 0.0;       //residential
            double l15_assumed = 15.0;      //education
            double l16_assumed = 15.0;      //special
            double l17_assumed = 0.0;       //carpark

            //Existing Buildings - heating performance in W/m2
            double k19_assumed = 64.0;      //production
            double k20_assumed = 80.0;      //office
            double k21_assumed = 80.0;      //research
            double k22_assumed = 40.0;      //residential
            double k23_assumed = 72.0;      //education
            double k24_assumed = 80.0;      //special

            //Existing Buildings - cooling performance in W/m2
            double l19_assumed = 45.0;      //production
            double l20_assumed = 25.0;      //office
            double l21_assumed = 25.0;      //research
            double l22_assumed = 0.0;       //residential
            double l23_assumed = 20.0;      //education
            double l24_assumed = 20.0;      //special		

            //Declaring variables
            double ac16_heating = 0;
            double ac27_heating = 0;

            if (j2_input == "New Buildings")
            {
                //Production heating loads in W
                double production_heating_load = 0.0;                                  
                double office_heating_load = k12_assumed * e5_area;              
                double research_heating_load = k13_assumed * g5_area;           
                double temp_residential_heating_load = k14_assumed * h5_area;    
                double residential_heating_load = k14_assumed * i5_area;         
                double education_heating_load = k15_assumed * j5_area;           
                double special_heating_load = k16_assumed * k5_area;             
                double carpark_heating_load = 0.0;                                      

                ac16_heating = production_heating_load + office_heating_load + research_heating_load + temp_residential_heating_load + residential_heating_load + education_heating_load + special_heating_load + carpark_heating_load;  //Total heating load in W for New Buildings
                ac27_heating = 0.0;
            }
            else if (j2_input == "Existing Buildings")
            {
                //Production heating loads in W
                double production_heating_load = k19_assumed * d18_area;             
                double office_heating_load = k20_assumed * e5_area;              
                double research_heating_load = k21_assumed * g5_area;           
                double temp_residential_heating_load = k22_assumed * h5_area;   
                double residential_heating_load = k22_assumed * i5_area;        
                double education_heating_load = k23_assumed * j5_area;          
                double special_heating_load = k24_assumed * k5_area;            
                double carpark_heating_load = 0.0;                                      

                ac16_heating = 0.0;
                ac27_heating = production_heating_load + office_heating_load + research_heating_load + temp_residential_heating_load + residential_heating_load + education_heating_load + special_heating_load + carpark_heating_load;      //Total heating load in W for Existing Buildings
            }

            double am36_syst = ac16_heating / 1000.0;       //heating demand max. power
            double ag38_syst = am36_syst / aj38_syst;       //underfloor heating New Buildings (kW)
            double ab38_syst = ag38_syst / ae38_syst;

            double am42_syst = 0.0;                         //hot water max. power (kW)
            double ag42_syst = am42_syst / aj42_syst;       //hot water system New Buildings (kW)

            double ae47_syst = ag42_syst / ae46_syst;       //heat pump hot water New Buildings power output 
            double ab42_syst = ag42_syst - ae47_syst;       //heat pump hot water New Buildings (kW)

            double am51_syst = ac27_heating / 1000.0;       //Heating demand for  Existing Buildings max. power output (kW)
            double ag51_syst = am51_syst / aj51_syst;       //convector(heater) Existing Buildings (kW)
            double ab51_syst = ag51_syst - ae51_syst;       //heat pump heating Existing Buildings (kW)

            double am60_syst = 0.0;                         //hot water max. power
            double ag60_syst = am60_syst / aj60_syst;       //hot water system Existing Buildings (kW)

            ae64_syst = ae63_syst / ae62_syst;              
            double ae65_syst = ag60_syst / ae64_syst;       //heat pump hot water Existing Buildings power output  (kW)
            double ab60_syst = ag60_syst - ae65_syst;       //heat pump hot water Existing Buildings (kW)

            double p43_syst = (ab38_syst + ab42_syst + ab51_syst + ab60_syst) / x43_syst;      //production requirements (kW)

            //Declaring variables
            double g70_syst = 0;   
            double g60_syst = 0;
            double l70_syst = 0;
                

            if (p44_syst != 0)
            {
                double c18_renewables = c10_renewables * c13_renewables * c12_renewables / 1000.0;  //power output - geothermal (kW)
                g70_syst = c18_renewables;                                  //heat pump geothermal (kW)
                l70_syst = g70_syst / (1 - 1.0 / j74_syst);          //heat pump geothermal (kW)
                double j11_renewables = 2.866;                              //solar energy (kWh/m2/a)
                double j12_renewables = j11_renewables / 24 * 1000.0;       //solar energy (W/m2)
                double j18_renewables = j12_renewables * j9_renewables * j13_renewables * j14_renewables / 1000.0;      //power output solar energy (kW)
                g60_syst = j18_renewables;                                  //solar energy (kW)
            }
            else
            {
                l70_syst = g70_syst / (1 - 1.0 / j74_syst);          //heat pump geothermal (kW)
            }

            double j61_syst = 0.7;      //(energy saved or power reduction) winter heat exchanger
            double l60_syst = g60_syst * j60_syst * j61_syst;      //heat exchanger (kW)

            //Declaring variables
            double g50_syst = 0;

            if (c18_2ndlayer == "Yes") 
            {
                double p9_renewables = p8_renewables / 24.0 / 3600.0;
                double p18_renewables = p7_renewables * p9_renewables * p10_renewables * p12_renewables * p14_renewables * 365.0 / 1000.0;      //waste water heat output
                g50_syst = p18_renewables;
            }
            else
            {
                g50_syst = 0;
            }

            double l50_syst = g50_syst / (1.0 - 1.0 / j54_syst);        //heat pump waste water (kW)
            double n46_syst = l70_syst + l60_syst + l50_syst;           //sustainable supply(kW)

            //Declaring variables

            double d25_syst = 0.9;
            int j30_syst = 6000;     //runtime BHKW (h/a)
            double j28_syst = 0;
            double l34_syst = 0;
            double l40_syst = 0;

            if (p44_syst - n47_syst >= 0)   
            {
                if (c23_2ndlayer_input == "Gas BHKW (KWKK)") 
                {
                    double p29_syst = p43_syst - n46_syst;              //production demand (kW)
                    double p30_syst = p44_syst - n47_syst;              //production demand (kW)
                    j28_syst = p30_syst * 1000.0 * d25_syst / j30_syst; //BHKW heat power
                    l34_syst = p29_syst - j28_syst;                     //peak load boilers
                }

                if (c23_2ndlayer_input == "AirHeatPump") 
                {
                    l40_syst = p43_syst - n46_syst;                     //generation demand for air heat pump
                }
            }

            double g8_ke = l34_syst + l40_syst;     //quantität
            int d8_ke = 110;                        //spezifischer   
            double j8_ke = d8_ke * g8_ke;           //central heating costs
            Console.WriteLine("j8_ke" + "   " + j8_ke);

            //calculate j9_ke - infrastructure EMSR costs (kg200 - central heating)
            int d9_ke = 55;                         //spezifischer 
            double g9_ke = p43_syst;                //quantität
            double j9_ke = d9_ke * g9_ke;           //EMSR costs

            Console.WriteLine("j9_ke" + "   " + j9_ke);

            //calculate j10_ke - infrastructure storage tank costs (kg200 - central heating)
            int d10_ke = 6640;                      //spezifischer
            double g10_ke = c9_2ndlayer;            //quantität
            double j10_ke = d10_ke * g10_ke;        //storage tank costs
            Console.WriteLine("j10_ke" + "   " + j10_ke);

            //calculate j11_ke - infrastructure mains connection costs (kg200 - central heating)
            int d11_ke = 3500;                      //spezifischer
            double g11_ke = c9_2ndlayer;            //quantität
            double j11_ke = d11_ke * g11_ke;        //mains connection costs
            Console.WriteLine("j11_ke" + "   " + j11_ke);

            //calculate j13_ke - infrastructure line DN100 costs (kg200 - central heating)
            int d13_ke = 215;                       //spezifischer
            double g13_ke = c8_2ndlayer * 0.09;     //quantität
            double j13_ke = d13_ke * g13_ke;        //low-ex network line DN100 costs
            Console.WriteLine("j13_ke" + "   " + j13_ke);

            //calculate j14_ke - infrastructure line DN150 costs (kg200 - central heating)
            int d14_ke = 286;                       //spezifischer
            double g14_ke = c8_2ndlayer * 0.5;      //quantität
            double j14_ke = d14_ke * g14_ke;        //low-ex network line DN150 costs
            Console.WriteLine("j14_ke" + "   " + j14_ke);

            //calculate j15_ke - infrastructure line DN300 costs (kg200 - central heating)
            double d15_ke = 550;                    //spezifischer
            double g15_ke = c8_2ndlayer * 0.41;     //quantität
            double j15_ke = d15_ke * g15_ke;        //low-ex network line DN300 costs
            Console.WriteLine("j15_ke" + "   " + j15_ke);

            //calculate j16_ke - infrastructure low-ex network kernbohrungen costs (kg200 - central heating)
            double d16_ke = 500;                    //spezifischer
            double g16_ke = c9_2ndlayer;            //kernbohrungen
            double j16_ke = d16_ke * g16_ke;        //low-ex network kernbohrungen costs
            Console.WriteLine("j16_ke" + "   " + j16_ke);

            //calculate j17_ke - infrastructure low-ex network demand for local heating network according to § 18 KWKG costs (kg200 - central heating)
            double d17_ke = 0.3;                    //spezifischer
            double j17_ke = (j13_ke + j14_ke + j15_ke + j16_ke) * d17_ke * (-1);    //low-ex network demand for local heating network according to § 18 KWKG costs
            Console.WriteLine("j17_ke" + "   " + j17_ke);

            //calculate j20_ke - energy source BHKW costs (kg200 - central heating)
            double d20_ke = 950;                    //spezifischer
            double g20_ke = j28_syst;               //quantität
            double j20_ke = d20_ke * g20_ke;        //Energy source BHKW costs
            Console.WriteLine("j20_ke" + "   " + j20_ke);

            //calculate j22_ke - energy source waste water costs (kg200 - central heating)
            double d22_ke = 800;                    //spezifischer
            double g22_ke = l50_syst;               //quantität
            double j22_ke = d22_ke * g22_ke;        //energy source waste water costs
            Console.WriteLine("j22_ke" + "   " + j22_ke);

            //calculate j24_ke - energy source demand costs (kg200 - central heating)
            double d24_ke = 0.3;                    //spezifischer
            double j24_ke = (-1) * d24_ke * j22_ke; //energy source demand costs
            Console.WriteLine("j24_ke" + "   " + j24_ke);

            //calculate j27_ke - energy source solar energy costs (kg200 - central heating)
            double d27_ke = 300;                    //spezifischer
            double g27_ke = j7_renewables;          //quantität
            double j27_ke = d27_ke * g27_ke;        //energy source solar energy costs
            Console.WriteLine("j27_ke" + "   " + j27_ke);

            //calculate j30_ke - energy source geothermal costs (kg200 - central heating)
            double d30_ke = 2400;                   //spezifischer
            double g30_ke = g70_syst;               //quantität
            double j30_ke = d30_ke * g30_ke;        //energy source geothermal costs
            Console.WriteLine("j30_ke" + "   " + j30_ke);

            //calculate j31_ke - energy source heat pump central costs (kg200 - central heating)
            double d31_ke = 8000 / 20;              //spezifischer
            double g31_ke = l50_syst + l60_syst + l70_syst + l40_syst;   //quantität
            double j31_ke = d31_ke * g31_ke;        //energy source heat pump central costs
            Console.WriteLine("j31_ke" + "   " + j31_ke);

            double sum_inv_kg200 = j8_ke + j9_ke + j10_ke + j11_ke + j13_ke + j14_ke + j15_ke + j16_ke + j17_ke + j20_ke + j22_ke + j24_ke + j27_ke + j30_ke + j31_ke;

            Console.WriteLine("sum_inv_kg200 = " + "   " + sum_inv_kg200);

            //Investment Costs [€] - KG400-Technology

            //calculate j34_ke - increasing heat pump decentralise costs (kg400 - decentralised facilities)
            double d34_ke = 6000.0 / 20.0;          //spezifischer
            double g34_ke = (ae48_syst + ae57_syst + ae66_syst) / 5000.0 * 1000.0; //quantität
            double j34_ke = d34_ke * g34_ke;        //increasing heat pump decentralise costs

            //calculate j35_ke - building TGA pro m2 BGF costs (kg400 - decentralised facilities)
            double d35_ke = 264.0;                  //spezifischer
            double d5_area = f2_input * 0.8;        //production NUF New Buildings 

            //Declaring variables
            double n15_area = 0;
            double n25_area = 0;

            if (j2_input == "New Buildings")
            {
                n15_area = d5_area + e5_area + g5_area + h5_area + i5_area + j5_area + k5_area + l5_area;
            }
            else
            {
                n25_area = d5_area + e5_area + g5_area + h5_area + i5_area + j5_area + k5_area + l5_area;
            }

            double g35_ke = n15_area;           //quantität
            double j35_ke = d35_ke * g35_ke;    //building TGA pro m2 BGF costs

            //calculate j38_ke - installation costs (electricity grid)
            double d38_ke = 120000.0;    //spezifischer

            //Declaring variables
            double g38_ke = 0;

            if (c10_2ndlayer == "Yes")     
            {
                g38_ke = 1;  //quantität
            }

            double j38_ke = d38_ke * g38_ke;     //installation costs

            double sum_inv_kg400 = j34_ke + j35_ke + j38_ke;
            Console.WriteLine("sum_inv_kg400 = " + "   " + sum_inv_kg400);

            //Investment Costs [€] - wind_power

            //calculate j44_ke - wind_power offshore costs (wind_power)
            double d44_ke = 3500.0;                     //spezifischer
            double g44_ke = c22_2ndlayer * 1000.0;      //quantität
            double inv_wind_power = d44_ke * g44_ke;    //wind_power offshore costs
            Console.WriteLine("inv_wind_power = " + "   " + inv_wind_power);


            //Investment Costs [€] - PV

            //calculate j48_ke - PV total costs (PV)
            double d48_ke = 1200.0;       //spezifischer

            //Declaring variables
            double q15_area = 0;
            double q25_area = 0;

            if (j2_input == "New Buildings")
            {
                q15_area = i2_input * c20_2ndlayer;     
            }
            else
            {
                q25_area = i2_input * c20_2ndlayer;      
            }

            double c8_solar = q25_area + q15_area;      //total effective area solar (m2)
            double c5_solar = 0.16;                     //PV efficiency solar
            double c9_solar = c8_solar * c5_solar;      //leistung solar (kW)
            double g48_ke = c9_solar;                   //mercedes PV preis (kWp)
            double inv_pv = d48_ke * g48_ke;            //total pv costs
            Console.WriteLine("inv_pv = " + "   " + inv_pv);

            //Investment Costs [€] - Battery

            //calculate j51_ke - battery tesia powerpacks costs
            double d51_ke = 55000.0;                                     //spezifischer
            double j4_solar = c25_2ndlayer;                              
            double g51_ke = Math.Round(j4_solar / 210.0 / 0.3) * 0.3;    //powerpacks
            double inv_battery = d51_ke * g51_ke;                        //battery costs
            Console.WriteLine("inv_battery = " + "   " + inv_battery);


            //Investment Costs [€] - Local Heat Storage

            //calculate j55_ke - local_heat_storage costs
            double d55_ke = 16000 / 10;                                                     //spezifischer
            double n4_local_heat_storage = c24_2ndlayer;  
            double radius_tank = 1.5;                                          
            double height = 7;                                                
            double n7_local_heat_storage = height * Math.PI * Math.Pow(radius_tank, 2);     //was originally n7_local_heat_storage = height* pi*radius_tank**2
            double g55_ke = n4_local_heat_storage * n7_local_heat_storage;                  //quantität (m3)
            double inv_local_heat_storage = d55_ke * g55_ke;                                //local_heat_storage costs
            Console.WriteLine("inv_local_heat_storage =" + "   " + inv_local_heat_storage);


            //Investment Costs [€] - Total

            //calculate Investment Costs total
            double inv_total = sum_inv_kg200 + sum_inv_kg400 + inv_wind_power + inv_pv + inv_battery + inv_local_heat_storage;
            Console.WriteLine("inv_total = " + "   " + inv_total);

            //Embodied THG Emission [tCO2e] - KG200

            //calculate s8_ke - infrastructure central heating embodied emissions (kg200 - central heating)
            double p8_ke = 1.53;                        //specific embodied emissions
            double n8_ke = n15_area + n25_area;         //quantität
            double s8_ke = p8_ke * n8_ke / 1000.0;      //infrastructure central heating embodied emissions

            //calculate s9_ke - infrastructure EMSR embodied emissions (kg200 - central heating)
            double p9_ke = 12.7;                        //specific embodied emissions
            double n9_ke = n8_ke;                       //quantität
            double s9_ke = p9_ke * n9_ke / 1000.0;      //infrastructure EMSR embodied emissions

            //calculate s10_ke - infrastructure storage tank embodied emissions (kg200 - central heating)
            double p10_ke = 3.5;                        //specific embodied emissions
            double n10_ke = g10_ke * 2000.0;            //quantität
            double s10_ke = p10_ke * n10_ke / 1000.0;   //infrastructure storage tank embodied emissions

            //calculate s12_ke - infrastructure low-ex netwerk embodied emissions (kg200 - central heating)
            double p12_ke = 1260.0;                     //specific embodied emissions
            double g12_ke = c8_2ndlayer;                
            double n12_ke = g12_ke;                     //quantität
            double s12_ke = p12_ke * n12_ke / 1000.0;   //infrastructure low-ex netwerk embodied emissions

            //calculate s20_ke - energy source BHKW embodied emissions (kg200 - central heating)
            double p20_ke = 2180.0 / 8.0 / 2.0;         //specific embodied emissions
            double n20_ke = g20_ke;                     //quantität
            double s20_ke = p20_ke * n20_ke / 1000.0;   //energy source BHKW embodied emissions

            //calculate s22_ke - energy source waste water embodied emissions (kg220 - central heating)
            double p22_ke = 2180.0 / 8.0;               //specific embodied emissions
            double n22_ke = g22_ke;                     //quantity
            double s22_ke = p22_ke * n22_ke / 1000.0;   //energy source waste water embodied emissions


            //calculate s27_ke - energy source solar energy embodied emissions (kg270 - central heating)
            double p27_ke = 155.0;                                  //specific embodied emissions
            j7_renewables = (p25_area + p15_area) * c21_2ndlayer;   
            j8_renewables = 1.0;                                    //utilisation factor solar energy
            double n27_ke = j7_renewables * j8_renewables;          //quantität
            double s27_ke = p27_ke * n27_ke / 1000.0;               //energy source solar energy embodied emissions

            //calculate s30_ke - energy source geothermal embodied emissions (kg300 - central heating)
            double p30_ke = 28.1;    //specific embodied emissions

            //Declaring variables
            double n30_ke = 0;
            if (n8_ke != 0)
            {
                n30_ke = c10_renewables * c13_renewables;   //quantität
            }
            double s30_ke = p30_ke * n30_ke / 1000.0;       //energy source geothermal embodied emissions

            //calculate s31_ke - energy source heat pump central embodied emissions
            double p31_ke = 2180.0 / 8.0;                               //specific embodied emissions
            double n31_ke = l50_syst + l60_syst + l70_syst + l40_syst;  //quantität
            double s31_ke = p31_ke * n31_ke / 1000.0;                   //energy source heat pump central embodied emissions
            double embodied_thg_em_kg200 = s8_ke + s9_ke + s10_ke + s12_ke + s20_ke + s22_ke + s27_ke + s30_ke + s31_ke;        //embodied_thg_em_kg200 - Embodied THG Emissions [tCO2e] - KG200
            Console.WriteLine("embodied_thg_em_kg200 = " + "   " + embodied_thg_em_kg200);

            //Embodied THG Emissions [tCO2e] - KG400-Technology

            //calculate s34_ke - increasing heat pump decentral embodied emissions
            double p34_ke = 2910.0 / 8.0;                                           //specific embodied emissions
            double n34_ke = (ae48_syst + ae57_syst + ae66_syst) / 5000.0 * 1000.0;  //quantität
            double s34_ke = p34_ke * n34_ke / 1000.0;                               //increasing heat pump decentral

            //calculate s35_ke - building embodied emissions

            double p35_ke = 3.07 + 6.06;                    //specific embodied emissions
            double n35_ke = n15_area + n25_area;            //quantität
            double s35_ke = p35_ke * n35_ke / 1000.0;       //building embodied emissions

            double embodied_thg_em_kg400 = s34_ke + s35_ke; //embodied_thg_em_kg400 - Embodied THG Emissions [tCO2e] - KG400-Technology
            Console.WriteLine("embodied_thg_em_kg400 = " + "   " + embodied_thg_em_kg400);

            // Embodied THG Emissions [tCO2e] - wind_power
            double p44_ke = 1600.0 / 3.0;           //spezifischer
            double n44_ke = c22_2ndlayer;           //quantität
            double s44_ke = p44_ke * n44_ke;        //embodied emissions - wind_power
            double embodied_thg_em_wind = s44_ke;
            Console.WriteLine("embodied_thg_em_wind = " + "   " + embodied_thg_em_wind);

            //Embodied THG Emissions [tCO2e] - PV
            double p48_ke = 2080.0;                     //spezifischer
            double n48_ke = c9_solar;                   //quantität
            double s48_ke = p48_ke * n48_ke / 1000.0;   //embodied emissions - PV total
            double embodied_thg_em_pv = s48_ke;
            Console.WriteLine("embodied_thg_em_pv = " + "   " + embodied_thg_em_pv);

            //Embodied THG Emissions [tCO2e] - Battery
            double p52_ke = 185.0;                      //spezifischer
            double n52_ke = c25_2ndlayer;               
            double s52_ke = p52_ke * n52_ke / 1000.0;   //embodied emissions battery alternative calculation
            double p51_ke = 80.6;                       //spezifischer
            double n51_ke = c25_2ndlayer;               
            double s51_ke = p51_ke * n51_ke / 1000.0;   //embodied emissions battery tesia powerpacks

            //Declaring variables
            double embodied_thg_em_bat = 0;

            if (c26_2ndlayer == "Lithium") 
            {
                embodied_thg_em_bat = s52_ke;       
            }
            else if (c26_2ndlayer == "Second-Life Lithium")
            {
                embodied_thg_em_bat = s51_ke;   
            }

            //Embodied THG Emissions [tCO2e] - Local Heat Storage
            double p55_ke = 540.0 / 4.0 * 3.51;                             //spezifischer, 3.51 - stahlblech verzinkt
            double n55_ke = n4_local_heat_storage * n7_local_heat_storage;  //quantität
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