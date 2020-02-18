#inputs
c2_input = 2942
d2_input = 0
b2_input = 11768
e2_input = 0
g2_input = 0
h2_input = 50
f2_input = 0
o2_input = 0
i2_input = 2942
j2_input = 'Neubau'
m2_input = 1
l2_input = 50
w2_input = 0
p2_input = 0.5
u2_input = 0
t2_input = 0
q2_input = 0.1
n2_input = 'Nein'
s2_input = 'Gas BHKW (KWKK)'
v2_input = 'Second-Life Lithium'
c5_2ndlayer = 188.288		#compute
c6_2ndlayer = 156.9066667	#compute
c18_2ndlayer = 'Ja'
c23_2ndlayer = o2_input
c9_2ndlayer = m2_input
c8_2ndlayer = l2_input
c22_2ndlayer = w2_input
c20_2ndlayer = p2_input
c25_2ndlayer = u2_input
c24_2ndlayer = t2_input
c23_2ndlayer_input = s2_input
c10_2ndlayer = n2_input
c21_2ndlayer = q2_input
c26_2ndlayer = v2_input


from math import pi, sqrt
import csv

kelvin = 273.15

#Investitionskosten [€] - KG200
#ke - kosten emissionen table

#calculate j8_ke - infrastruktur heizzentrale kosten (kg200 - heizzentrale)

#for neubau
e12_annahmen = 45 		#e12_annahmen - neubau heizen buro energiebedarf in kWh/m2/a
e13_annahmen = 45 		#e13_annahmen - neubau heizen forschung energiebedarf in kWh/m2/a
e14_annahmen = 25 		#e14_annahmen - neubau heizen wohnen energiebedarf in kWh/m2/a
e15_annahmen = 45		#e15_annahmen - neubau heizen bildung energiebedarf in kWh/m2/a
e16_annahmen = 45 		#e16_annahmen - neubau heizen sonder energiebedarf in kWh/m2/a
e17_annahmen = 0 		#e17_annahmen - neubau heizen parken energiebedarf in kWh/m2/a

#for bestand
e19_annahmen = 80 		#e19_annahmen - bestand heizen production energiebedarf in kWh/m2/a
e20_annahmen = 100		#e20_annahmen - bestand heizen buro energiebedarf in kWh/m2/a
e21_annahmen = 100 		#e21_annahmen - bestand heizen forschung energiebedarf in kWh/m2/a
e22_annahmen = 90 		#e22_annahmen - bestand heizen wohnen energiebedarf in kWh/m2/a
e23_annahmen = 90 		#e23_annahmen - bestand heizen bildung energiebedarf in kWh/m2/a
e24_annahmen = 80 		#e24_annahmen - bestand heizen sonder energiebedarf in kWh/m2/a
	
e5_flachen = 0.8*c2_input		#e5_flachen - flachen buro			#INPUT!!!!!!
g5_flachen = 0.8*d2_input 		#g5_flachen - flachen forschung		#INPUT!!!!!!
h5_flachen = 0					#h5_flachen - flachen temp wohnen	#INPUT!!!!!!
i5_flachen = 0.8*b2_input		#i5_flachen - flachen wohnen		#INPUT!!!!!!
j5_flachen = 0.8*e2_input 		#j5_flachen - flachen bildung		#INPUT!!!!!!
k5_flachen = 0.8*g2_input		#k5_flachen - flachen sonder		#INPUT!!!!!!
l5_flachen = 0.8*h2_input		#l5_flachen - flachen parken		#INPUT!!!!!!
d18_flachen = 0.8*f2_input		#d18_flachen - flachen production	#INPUT!!!!!!

#for neubau
f12_annahmen = 5		#f12_annahmen - neubau warmwasser buro energiebedarf in kWh/m2/a
f13_annahmen = 5		#f13_annahmen - neubau warmwasser forschung energiebedarf in kWh/m2/a
f14_annahmen = 20		#f14_annahmen - neubau warmwasser wohnen energiebedarf in kWh/m2/a
f15_annahmen = 10		#f15_annahmen - neubau warmwasser bildung energiebedarf in kWh/m2/a
f16_annahmen = 5		#f16_annahmen - neubau warmwasser sonder energiebedarf in kWh/m2/a
f17_annahmen = 0		#f17_annahmen - neubau warmwasser parken energiebedarf in kWh/m2/a

#for bestand
f19_annahmen = 5		#f19_annahmen - bestand warmwasser production energiebedarf in kWh/m2/a
f20_annahmen = 20		#f20_annahmen - bestand warmwasser buro energiebedarf in kWh/m2/a
f21_annahmen = 20		#f21_annahmen - bestand warmwasser forschung energiebedarf in kWh/m2/a
f22_annahmen = 20 		#f22_annahmen - bestand warmwasser wohnen energiebedarf in kWh/m2/a
f23_annahmen = 20 		#f23_annahmen - bestand warmwasser bildung energiebedarf in kWh/m2/a
f24_annahmen = 20 		#f24_annahmen - bestand warmwasser sonder energiebedarf in kWh/m2/a

if j2_input == 'Neubau':
	
	#heizen
	production_heizen = 0
	buro_heizen = e12_annahmen*e5_flachen
	forschung_heizen = e13_annahmen*g5_flachen
	temp_wohnen_heizen = e14_annahmen*h5_flachen
	wohnen_heizen = e14_annahmen*i5_flachen
	bildung_heizen = e15_annahmen*j5_flachen
	sonder_heizen = e16_annahmen*k5_flachen
	parken_heizen = e17_annahmen*l5_flachen
	
	n16_heizen = buro_heizen + forschung_heizen + temp_wohnen_heizen + wohnen_heizen + bildung_heizen + sonder_heizen + parken_heizen		#n16_heizen - gesamt for heizen
	n27_heizen = 0
	
	#warmwasser
	production_warmwasser = 0
	buro_warmwasser = f12_annahmen*e5_flachen
	forschung_warmwasser = f13_annahmen*g5_flachen
	temp_wohnen_warmwasser = f14_annahmen*h5_flachen
	wohnen_warmwasser = f14_annahmen*i5_flachen
	bildung_warmwasser = f15_annahmen*j5_flachen
	sonder_warmwasser = f16_annahmen*k5_flachen
	parken_warmwasser = 0
	
	n16_warmwasser = production_warmwasser + buro_warmwasser + forschung_warmwasser + temp_wohnen_warmwasser + wohnen_warmwasser + bildung_warmwasser + sonder_warmwasser + parken_warmwasser 	#n16_warmwasser - gesamt for warmwasser
	n27_warmwasser = 0
	
	
elif j2_input == 'Bestand':

	production_heizen = e19_annahmen*d18_flachen
	buro_heizen = e20_annahmen*e5_flachen
	forschung_heizen = e21_annahmen*g5_flachen
	temp_wohnen_heizen = e22_annahmen*h5_flachen
	wohnen_heizen = e22_annahmen*i5_flachen
	bildung_heizen = e23_annahmen*j5_flachen
	sonder_heizen = e24_annahmen*k5_flachen
	parken_heizen = 0*l5_flachen
	
	n16_heizen = 0
	n27_heizen = production_heizen + buro_heizen + forschung_heizen + temp_wohnen_heizen + wohnen_heizen + bildung_heizen + sonder_heizen + parken_heizen		#n16_heizen - gesamt for heizen
	
	production_warmwasser = f19_annahmen*d18_flachen 
	buro_warmwasser = f20_annahmen*e5_flachen
	forschung_warmwasser = f21_annahmen*g5_flachen
	temp_wohnen_warmwasser = f22_annahmen*h5_flachen
	wohnen_warmwasser = f22_annahmen*i5_flachen
	bildung_warmwasser = f23_annahmen*j5_flachen
	sonder_warmwasser = f24_annahmen*k5_flachen
	parken_warmwasser = 0
	
	n16_warmwasser = 0
	n27_warmwasser = production_warmwasser + buro_warmwasser + forschung_warmwasser + temp_wohnen_warmwasser + wohnen_warmwasser + bildung_warmwasser + sonder_warmwasser + parken_warmwasser 	#n16_warmwasser - gesamt for warmwasser

aj38_syst = 0.98        #aj38_syst - fussbodenheizung neubau (effizienz)
am38_syst = n16_heizen/1000     
ag39_syst = am38_syst/aj38_syst     #am38_syst - heizenergie (MWh/a), aj38_syst - fussbodenheizung neubau (effizienz)
ae38_syst = 0.95        #warmetausher effizienz
ab39_syst = ag39_syst/ae38_syst     #ag39_syst - fussbodenheizung neubau, ae38_syst - warmetausher (effizienz)





am43_syst = n16_warmwasser/1000
aj42_syst = 0.95        #warmwasser system neubau (effizienz)
ag43_syst = am43_syst/aj42_syst     #am43_syst - warmwasser (warmwasserenergie), aj42_syst - warmwasser system neubau (effizienz)
ae45_syst = 0.5     #efficiency
ae43_syst = 70      #temperatur warmwasser neubau
ae42_syst = 40      #temperatur (low-ex netzwerk)
ae44_syst = (ae43_syst + kelvin)/((ae43_syst + kelvin) - (ae42_syst + kelvin))      #ae44_syst - warmepumpe warmwasser neubau COPideal
ae46_syst = ae45_syst * ae44_syst   #ae46_syst - warmepumpe warmwasser neubau COPreal 
ae48_syst = ag43_syst/ae46_syst     #ag43_syst - warmwasser system neubau (MWh/a)
ab43_syst = ag43_syst/ae48_syst     #ag43_syst - warmwasser system neubau, ae48_syst - warmepumpe warmwasser neubau (strombedarf)



am52_syst = n27_heizen/1000
aj51_syst = 0.95        #konvektoren bestand (effizienz)
ag52_syst = am52_syst/aj51_syst     #am52_syst - heizenergie, aj51_syst - konvektoren bestand (effizienz)


ae54_syst = 0.5     #warmepumpe heizen bestand (efficiency)
ae51_syst = 40      #Tc
ae52_syst = 50      #Th
ae53_syst = (ae52_syst + kelvin)/((ae52_syst + kelvin) - (ae51_syst + kelvin))
ae55_syst = ae54_syst*ae53_syst     #ae55_syst - COPreal
ae57_syst = ag52_syst/ae55_syst     #ag52_syst - konvektoren bestand, ae57_syst - warmepumpe heizen bestand

ab52_syst = ag52_syst - ae57_syst       #ag52_syst - konvektoren bestand, ae57_syst - warmepumpe heizen bestand (strombedarf)

am61_syst = n27_warmwasser/1000     #am61_syst - warmwasserenergie bestand
aj60_syst = 0.95        #warmwasser system bestand (effizienz)
ag61_syst = am61_syst/aj60_syst     #ag61_syst - warmwasser system bestand, am61_syst - warmwasserenergie bestand, aj60_syst - warmwasser system bestand (effizienz)
ae63_syst = 0.5     #ae63_syst - warmepumpe warmwasser bestand (efficiency)
ae60_syst = 40      #Tc
ae61_syst = 70      #Th
ae62_syst = (ae61_syst + kelvin)/((ae61_syst + kelvin) - (ae60_syst + kelvin))
ae64_syst = ae63_syst*ae62_syst     #ae64_syst - COPreal, ae63_syst - warmepumpe warmwasser bestand (efficiency), ae62_syst - COPideal
ae66_syst = ag61_syst/ae64_syst     #ae64_syst - COPreal
ab61_syst = ag61_syst - ae66_syst      #warmwasser system bestand, ae66_syst - warmepumpe warmwasser bestand (strombedarf)

x43_syst = 0.9      #low-ex netzwerk (effizienz)

p44_syst = (ab39_syst + ab43_syst + ab52_syst + ab61_syst)/x43_syst     #ab39_syst - warmetausher, ab43_syst - warmepumpe warmwasser neubau, ab52_syst - warmepumpe heizen bestand, ab61_syst - warmepumpe warmwasser bestand, x43_syst - effizienz (low-ex netwerk)

c7_erneuerbare = o2_input       #c7_erneuerbare - vergfugbare flache    INPUT!!!!!
c8_erneuerbare = 6      #radius
c9_erneuerbare = (pi*sqrt(3)/6*c7_erneuerbare)/(pi*c8_erneuerbare**2)   #c9_erneuerbare - maximale anzahl
c6_erneuerbare = 1 		#c6_erneuerbare - geo factor
c10_erneuerbare = c9_erneuerbare*c6_erneuerbare   #c10_erneuerbare - anzahl sonden
c13_erneuerbare = 100      #c13_erneuerbare - lange der sonde
c12_erneuerbare = 45		#c12_erneuerbare - spezifische warmekapazitat (W/m)
c14_erneuerbare = 1800		#c14_erneuerbare - load hours (h/a)

if p44_syst == 0:
    g71_syst = 0
else:
	c19_erneuerbare = c10_erneuerbare*c13_erneuerbare*c12_erneuerbare*c14_erneuerbare/1000000		#c19_erneuerbare - geothermie warmepotential
	g71_syst = c19_erneuerbare#(MWh/a)

j73_syst = 0.5       #warmepumpe geothermie (efficiency)
j71_syst = 40        #warmepumpe geothermie Th
j70_syst = 15        #warmepumpe geothermie Tc                     
j72_syst = (j71_syst + kelvin)/((j71_syst + kelvin) - (j70_syst + kelvin))      #j72_syst - COPideal
j74_syst = j73_syst * j72_syst      #j74_syst - COPreal

l71_syst = g71_syst/(1 - 1/j74_syst)        #j74_syst - COPreal

if p44_syst == 0:
    g61_syst = 0        #g61_syst - solarthermie (MWh/a)
else:
	j13_erneuerbare = 0.65		#j13_erneuerbare - solarthermie efficiency
	j10_erneuerbare = 1046		#j10_erneuerbare - solarthermie global horizontal irradiation (kWh/m2/a)
	if j2_input == 'Neubau':
		p15_flachen = i2_input			#p15_flachen - dachflache INPUT!!!!!!
		p25_flachen = 0					#p25_flachen - dachflache
	elif j2_input == 'Bestand':
		p15_flachen = 0					#p15_flachen - dachflache
		p25_flachen = i2_input			#p25_flachen - dachflache INPUT!!!!!!
	j7_erneuerbare = (p25_flachen + p15_flachen)*q2_input		#j7_erneuerbare - solarthermie flache (m2)
	j8_erneuerbare = 1 				#j8_erneuerbare - solarthermie nutzungsfaktor
	j9_erneuerbare = j7_erneuerbare*j8_erneuerbare
	j14_erneuerbare = 0.95			#j14_erneuerbare - solarthermie nutzungsfaktor
	j19_erneuerbare = j13_erneuerbare*j10_erneuerbare*j9_erneuerbare*j14_erneuerbare/1000		#j19_erneuerbare - solarthermie warmepotential (MWh/a)
	g61_syst = j19_erneuerbare      #g61_syst - solarthermie (MWh/a)
j60_syst = 0.95     #warmetausher effizienz
j62_syst = 1        #warmetausher utilization    

l61_syst = g61_syst * j60_syst * j62_syst       #l61_syst - warmetausher (MWh/a)

if c18_2ndlayer == 'Ja':
	p7_erneuerbare = c5_2ndlayer + c6_2ndlayer 		#p7_erneuerbare - abwasserwarme personen	INPUT!!!!!!!!
	p8_erneuerbare = 80			#p8_erneuerbare - abwassermenge (I/d)
	p10_erneuerbare = 4.19		#p10_erneuerbare - abwasserwarme spezifischer energiegehalt wasser (kJ/kgWasser/K)
	p12_erneuerbare = 10		#p12_erneuerbare - abwasserwarme temperaturdifferenz warmetausher (K)
	p13_erneuerbare = 1/3600	#p13_erneuerbare - abwasserwarme umrechnung (kWh/kJ)
	p14_erneuerbare = 0.6		#p14_erneuerbare - abwasserwarme effizienz warmetausher
	p19_erneuerbare = p7_erneuerbare*p8_erneuerbare*p10_erneuerbare*p12_erneuerbare*p13_erneuerbare*p14_erneuerbare*365/1000		#p19_erneuerbare - abwasserwarme warmepotential
	g51_syst = p19_erneuerbare      #g51_syst - (MWh/a)
else:
    g51_syst = 0
    
j53_syst = 0.5      #warmepumpe abwasser (efficiency)
j51_syst = 40       #warmepumpe abwasser Th
j50_syst = 15       #warmepumpe abwasser Tc
j52_syst = (j51_syst + kelvin)/((j51_syst + kelvin) - (j50_syst + kelvin))      #j52_syst - COPideal
j54_syst = j53_syst*j52_syst       #j54_syst - warmepumpe abwasser COPreal
l51_syst = g51_syst/(1 - 1/j54_syst)        #l51_syst - warmepumpe abwasser (MWh/a)
n47_syst = l71_syst + l61_syst + l51_syst   #n47_syst - sustainable supply (MWh/a), l71_syst - warmepumpe geothermie (MWh/a), l61_syst - warmetausher (MWh/a), l51_syst - warmepumpe abwasser (MWh/a)


#for neubau
k12_annahmen = 55		#k12_annahmen - neubau heizen buro leistung in W/m2
k13_annahmen = 55		#k13_annahmen - neubau heizen forschung leistung in W/m2
k14_annahmen = 28 		#k14_annahmen - neubau heizen wohnen leistung in W/m2
k15_annahmen = 55		#k15_annahmen - neubau heizen bildung leistung in W/m2
k16_annahmen = 55		#k16_annahmen - neubau heizen sonder leistung in W/m2
k17_annahmen = 0		#k17_annahmen - neubau heizen parken leistung in W/m2

l12_annahmen = 15		#l12_annahmen - neubau kuhlen buro leistung in W/m2
l13_annahmen = 25		#l13_annahmen - neubau kuhlen forschung leistung in W/m2
l14_annahmen = 0 		#l14_annahmen - neubau kuhlen wohnen leistung in W/m2
l15_annahmen = 15		#l15_annahmen - neubau kuhlen bildung leistung in W/m2
l16_annahmen = 15		#l16_annahmen - neubau kuhlen sonder leistung in W/m2
l17_annahmen = 0 		#l17_annahmen - neubau kuhlen parken leistung in W/m2

#for bestand
k19_annahmen = 64		#k19_annahmen - bestand heizen production leistung in W/m2
k20_annahmen = 80		#k20_annahmen - bestand heizen buro leistung in W/m2
k21_annahmen = 80		#k21_annahmen - bestand heizen forschung leistung in W/m2
k22_annahmen = 40		#k22_annahmen - bestand heizen wohnen leistung in W/m2
k23_annahmen = 72		#k23_annahmen - bestand heizen bildung leistung in W/m2
k24_annahmen = 80		#k24_annahmen - bestand heizen sonder leistung in W/m2

l19_annahmen = 45		#l19_annahmen - bestand kuhlen production leistung in W/m2
l20_annahmen = 25		#l20_annahmen - bestand kuhlen buro leistung in W/m2
l21_annahmen = 25 		#l21_annahmen - bestand kuhlen forschung leistung in W/m2
l22_annahmen = 0 		#l22_annahmen - bestand kuhlen wohnen leistung in W/m2
l23_annahmen = 20		#l23_annahmen - bestand kuhlen bildung leistung in W/m2
l24_annahmen = 20 		#l24_annahmen - bestand kuhlen sonder leistung in W/m2		

if j2_input == 'Neubau':
	prod_heizen_heizlast = 0		#s7_heizen - production heizen heizlast in W
	buro_heizen_heizlast = k12_annahmen*e5_flachen		#t7_heizen - buro heizen heizlast in W
	forschung_heizen_heizlast = k13_annahmen*g5_flachen		#v7_heizen - forschung heizen heizlast in W
	temp_w_heizen_heizlast = k14_annahmen*h5_flachen		#w7_heizen - temp wohnen heizen heizlast in W
	wohnen_heizen_heizlast = k14_annahmen*i5_flachen		#x7_heizen - wohnen heizen heizlast in W
	bildung_heizen_heizlast = k15_annahmen*j5_flachen		#y7_heizen - bildung heizen heizlast in W
	sonder_heizen_heizlast = k16_annahmen*k5_flachen		#z7_heizen - sonder heizen heizlast in W
	parken_heizen_heizlast = 0		#aa7_heizen - parken heizen heizlast in W
	
	ac16_heizen = prod_heizen_heizlast + buro_heizen_heizlast + forschung_heizen_heizlast + temp_w_heizen_heizlast + wohnen_heizen_heizlast + bildung_heizen_heizlast + sonder_heizen_heizlast + parken_heizen_heizlast		#ac16_heizen - gesamt of heizlast in W for neubau
	ac27_heizen = 0
	
elif j2_input == 'Bestand':
	prod_heizen_heizlast = k19_annahmen*d18_flachen		#s20_heizen - production heizen heizlast in W
	buro_heizen_heizlast = k20_annahmen*e5_flachen		#t20_heizen - buro heizen heizlast in W
	forschung_heizen_heizlast = k21_annahmen*g5_flachen		#v20_heizen - forschung heizen heizlast in W
	temp_w_heizen_heizlast = k22_annahmen*h5_flachen		#w20_heizen - temp wohnen heizen heizlast in W
	wohnen_heizen_heizlast = k22_annahmen*i5_flachen		#x20_heizen - wohnen heizen heizlast in W
	bildung_heizen_heizlast = k23_annahmen*j5_flachen		#y20_heizen - bildung heizen heizlast in W
	sonder_heizen_heizlast = k24_annahmen*k5_flachen		#z20_heizen - sonder heizen heizlast in W
	parken_heizen_heizlast = 0		#aa20_heizen - parken heizen heizlast in W
	
	ac16_heizen = 0
	ac27_heizen = prod_heizen_heizlast + buro_heizen_heizlast + forschung_heizen_heizlast + temp_w_heizen_heizlast + wohnen_heizen_heizlast + bildung_heizen_heizlast + sonder_heizen_heizlast + parken_heizen_heizlast		#ac16_heizen - gesamt of heizlast in W for bestand

am36_syst = ac16_heizen/1000        #am36_syst - heizbedarf max. Leistung
ag38_syst = am36_syst/aj38_syst     #ag38_syst - fussbodenheizung neubau (kW)
ab38_syst = ag38_syst/ae38_syst     #ab38_syst - warmetausher (kW)

am42_syst = 0       #warmwasser max. Leistung (kW)
ag42_syst = am42_syst/aj42_syst     #ag42_syst - warmwasser system neubau (kW)

ae47_syst = ag42_syst/ae46_syst     #ae47_syst - warmepumpe warmwasser neubau stromleistung
ab42_syst = ag42_syst - ae47_syst   #ab42_syst - warmepumpe warmwasser neubau (kW)


am51_syst = ac27_heizen/1000    #am51_syst - heizbedarf bestand max. Leistung (kW)
ag51_syst = am51_syst/aj51_syst   #ag51_syst - konvektoren bestand (kW), aj51_syst - konvektoren bestand effizienz
ab51_syst = ag51_syst - ae51_syst       #ab51_syst - warmepumpe heizen bestand (kW)


am60_syst = 0       #warmwasser max. Leistung
aj60_syst = 0.95    #warmwasser system bestand effizienz
ag60_syst = am60_syst/aj60_syst         #ag60_syst - warmwasser system bestand (kW)

ae64_syst = ae63_syst/ae62_syst     #ae64_syst - warmepumpe warmwasser bestand COPreal
ae65_syst = ag60_syst/ae64_syst     #ae65_syst - warmepumpe warmwasser bestand stromleistung (kW)
ab60_syst = ag60_syst - ae65_syst       #ab60_syst - warmepumpe warmwasser bestand (kW)

p43_syst = (ab38_syst + ab42_syst + ab51_syst + ab60_syst)/x43_syst      #p43_syst - erzeugungsbedarf (kW)

if p44_syst == 0:
    g70_syst = 0        #g70_syst - warmepumpe geothermie (kW)
else:
	c18_erneuerbare = c10_erneuerbare*c13_erneuerbare*c12_erneuerbare/1000	#c18_erneuerbare - leistung geothermie (kW)
	g70_syst = c18_erneuerbare      #g70_syst - (kW)

l70_syst = g70_syst/(1 - 1/j74_syst)        #l70_syst - warmepumpe geothermie (kW)

if p44_syst == 0:
    g60_syst = 0                    #g60_syst - solarthermie (kW)
else:
	j11_erneuerbare = 2.866		#j11_erneuerbare - solarthermie (kWh/m2/a)
	j12_erneuerbare = j11_erneuerbare/24*1000		#j12_erneuerbare - solarthermie (W/m2)
	j18_erneuerbare = j12_erneuerbare*j9_erneuerbare*j13_erneuerbare*j14_erneuerbare/1000		#j18_erneuerbare - leistung solarthermie (kW)
	g60_syst = j18_erneuerbare      #g60_syst - solarthermie (kW)
j61_syst = 0.7      #leistungsreduktion winter warmetausher
l60_syst = g60_syst*j60_syst*j61_syst       #l60_syst - warmetausher (kW)

if c18_2ndlayer == 'Ja':			#c18_2ndlayer - INPUT!!!!!!!
	p9_erneuerbare = p8_erneuerbare/24/3600
	p18_erneuerbare = p7_erneuerbare*p9_erneuerbare*p10_erneuerbare*p12_erneuerbare*p14_erneuerbare*365/1000		#p18_erneuerbare - leistung abwasserwarme
	g50_syst = p18_erneuerbare
else:
    g50_syst = 0
l50_syst = g50_syst/(1 - 1/j54_syst)        #l50_syst - warmepumpe abwasser (kW)
n46_syst = l70_syst + l60_syst + l50_syst       #sustainable supply
if p44_syst - n47_syst < 0:     #p44_syst - matching-erzrugungsbedarf (MWh/a), n47_syst - sustainable supply (MWh/a)
    p29_syst = 0
else:
    if c23_2ndlayer_input == 'Gas BHKW (KWKK)':     #INPUT!!!!!!
        p29_syst = p43_syst - n46_syst      #p43_syst - erzeugungsbedarf (kW), n46_syst - sustainable supply (kW)
    else:
        p29_syst = 0

if p44_syst - n47_syst < 0:
    p30_syst = 0        #p30_syst - erzeugungsbedarf (kW)
else:
    if c23_2ndlayer_input == 'Gas BHKW (KWKK)':     #INPUT!!!!!!!
        p30_syst = p44_syst - n47_syst
    else:
        p30_syst = 0
d25_syst = 0.9
j30_syst = 6000     #laufzeit BHKW (h/a)
j28_syst = p30_syst*1000*d25_syst/j30_syst      #j28_syst - BHKW Leistung warme
l34_syst = p29_syst - j28_syst  #p29_syst - erzeugungsbedarf (kW), j29_syst - BHKW Leistung Wärme
if p44_syst - n47_syst < 0:
    l40_syst = 0
else:
    if c23_2ndlayer_input == 'Luftwaermepumpe':
        l40_syst = p43_syst - n46_syst
    else:
        l40_syst = 0
g8_ke = l34_syst + l40_syst     #l34_syst - spitzenlastkessel, l40_syst - erzeugungsbedarf luftwärmepumpe
d8_ke = 110
j8_ke = d8_ke*g8_ke     #j8_ke - heizzentrale kosten, d8_ke - spezifischer, g8_ke - quantität
print('j8_ke',j8_ke)
#calculate j9_ke - infrastruktur EMSR kosten (kg200 - heizzentrale)
d9_ke = 55      #d9_ke - spezifischer
g9_ke = p43_syst        #g9_ke - quantität
j9_ke = d9_ke*g9_ke     #j9_ke - EMSR kosten, d9_ke - spezifischer, g9_ke - quantität
print('j9_ke',j9_ke)
#calculate j10_ke - infrastruktur Pufferspeicher kosten (kg200 - heizzentrale)
d10_ke = 6640       #d10_ke - spezifischer
g10_ke = c9_2ndlayer        #INPUT!!!!!!
j10_ke = d10_ke*g10_ke     #j10_ke - Pufferspeicher kosten, d10_ke - spezifischer, g10_ke - quantität
print('j10_ke',j10_ke)
#calculate j11_ke - infrastruktur HA-Stationen kosten (kg200 - heizzentrale)
d11_ke = 3500       #d11_ke - spezifischer
g11_ke = c9_2ndlayer    #INPUT!!!!!
j11_ke = d11_ke*g11_ke     #j11_ke - HA-Stationen kosten, d11_ke - spezifischer, g11_ke - quantität
print('j11_ke',j11_ke)
#calculate j13_ke - infrastruktur trasse DN100 kosten (kg200 - heizzentrale)
d13_ke = 215        #d13_ke - spezifischer
g13_ke = c8_2ndlayer*0.09   #INPUT!!!!!!
j13_ke = d13_ke*g13_ke     #j13_ke - low-ex netzwerk trasse DN100 kosten, d13_ke - spezifischer, g13_ke - quantität
print('j13_ke',j13_ke)
#calculate j14_ke - infrastruktur trasse DN150 kosten (kg200 - heizzentrale)
d14_ke = 286        #d14_ke - spezifischer
g14_ke = c8_2ndlayer*0.5   #INPUT!!!!!!
j14_ke = d14_ke*g14_ke     #j14_ke - low-ex netzwerk trasse DN150 kosten, d14_ke - spezifischer, g14_ke - quantität
print('j14_ke',j14_ke)
#calculate j15_ke - infrastruktur trasse DN300 kosten (kg200 - heizzentrale)
d15_ke = 550        #d15_ke - spezifischer
g15_ke = c8_2ndlayer*0.41   #INPUT!!!!!!
j15_ke = d15_ke*g15_ke     #j15_ke - low-ex netzwerk trasse DN300 kosten, d15_ke - spezifischer, g15_ke - quantität
print('j15_ke',j15_ke)
#calculate j16_ke - infrastruktur low-ex netzwerk kernbohrungen kosten (kg200 - heizzentrale)
d16_ke = 500        #d16_ke - spezifischer
g16_ke = c9_2ndlayer   #INPUT!!!!!!
j16_ke = d16_ke*g16_ke     #j16_ke - low-ex netzwerk kernbohrungen kosten, d16_ke - spezifischer, g16_ke - quantität
print('j16_ke',j16_ke)
#calculate j17_ke - infrastruktur low-ex netzwerk forderung nahwarmenetz nach § 18 KWKG kosten (kg200 - heizzentrale)
d17_ke = 0.3        #d17_ke - spezifischer
j17_ke = (j13_ke + j14_ke + j15_ke + j16_ke)*d17_ke*(-1)    #j17_ke - low-ex netzwerk forderung nahwarmenetz nach § 18 KWKG kosten, d17_ke - spezifischer, g17_ke - quantität
print('j17_ke',j17_ke)
#calculate j20_ke - energiequelle BHKW kosten (kg200 - heizzentrale)
d20_ke = 950        #d20_ke - spezifischer
g20_ke = j28_syst   #g20_ke - quantität
j20_ke = d20_ke*g20_ke     #j20_ke - energiequelle BHKW kosten, d20_ke - spezifischer, g20_ke - quantität
print('j20_ke',j20_ke)
#calculate j22_ke - energiequelle abwasserwarme kosten (kg200 - heizzentrale)
d22_ke = 800        #d22_ke - spezifischer
g22_ke = l50_syst   #g22_ke - quantität
j22_ke = d22_ke*g22_ke     #j22_ke - energiequelle abwasserwarme kosten, d22_ke - spezifischer, g22_ke - quantität
print('j22_ke',j22_ke)
#calculate j24_ke - energiequelle forderung kosten (kg200 - heizzentrale)
d24_ke = 0.3        #d24_ke - spezifischer
j24_ke = (-1)*d24_ke*j22_ke     #j24_ke - energiequelle forderung kosten, d24_ke - spezifischer, j22_ke - energiequelle abwasserwarme kosten
print('j24_ke',j24_ke)
#calculate j27_ke - energiequelle solarthermie kosten (kg200 - heizzentrale)
d27_ke = 300        #d27_ke - spezifischer
g27_ke = j7_erneuerbare   #g27_ke - quantität
j27_ke = d27_ke*g27_ke     #j27_ke - energiequelle solarthermie kosten, d27_ke - spezifischer, g27_ke - quantität
print('j27_ke',j27_ke)
#calculate j30_ke - energiequelle geothermie kosten (kg200 - heizzentrale)
d30_ke = 2400        #d30_ke - spezifischer
g30_ke = g70_syst   #g30_ke - quantität
j30_ke = d30_ke*g30_ke     #j30_ke - energiequelle geothermie kosten, d30_ke - spezifischer, g30_ke - quantität
print('j30_ke',j30_ke)
#calculate j31_ke - energiequelle warmepumpe zentral kosten (kg200 - heizzentrale)
d31_ke = 8000/20        #d31_ke - spezifischer
g31_ke = l50_syst + l60_syst + l70_syst + l40_syst   #g31_ke - quantität
j31_ke = d31_ke*g31_ke     #j31_ke - energiequelle warmepumpe zentral kosten, d31_ke - spezifischer, g31_ke - quantität
print('j31_ke',j31_ke)
sum_inv_kg200 = j8_ke + j9_ke + j10_ke + j11_ke + j13_ke + j14_ke + j15_ke + j16_ke + j17_ke + j20_ke + j22_ke + j24_ke + j27_ke + j30_ke + j31_ke

print('sum_inv_kg200 = ', sum_inv_kg200)

#Investitionskosten [€] - KG400-Technik

#calculate j34_ke - anhebung warmepumpen dezentral kosten (kg400 - dezentrale einrichtungen)
d34_ke = 6000/20    #d34_ke - spezifischer
g34_ke = (ae48_syst + ae57_syst + ae66_syst)/5000*1000
j34_ke = d34_ke*g34_ke     #j34_ke - anhebung warmepumpen dezentral kosten, d34_ke - spezifischer, g34_ke - quantität

#calculate j35_ke - gebaude TGA pro m2 BGF kosten (kg400 - dezentrale einrichtungen)
d35_ke = 264    #d35_ke - spezifischer
d5_flachen = f2_input*0.8		#d5_flachen - production NUF neubau INPUT!!!!!!!!
if j2_input == 'Neubau':
	n15_flachen = d5_flachen + e5_flachen + g5_flachen + h5_flachen +i5_flachen + j5_flachen + k5_flachen + l5_flachen
	n25_flachen = 0
else:
	n25_flachen = d5_flachen + e5_flachen + g5_flachen + h5_flachen +i5_flachen + j5_flachen + k5_flachen + l5_flachen
	n15_flachen = 0
g35_ke = n15_flachen    #g35_ke - quantität
j35_ke = d35_ke*g35_ke     #j35_ke - gebaude TGA pro m2 BGF kosten, d35_ke - spezifischer, g35_ke - quantität

#calculate j38_ke - anschluss kosten (stromnetz)
d38_ke = 120000    #d38_ke - spezifischer
if c10_2ndlayer == 'Ja':		#INPUT!!!!!!!!!
    g38_ke = 1  #g38_ke - quantität
else:
    g38_ke = 0
j38_ke = d38_ke*g38_ke     #j38_ke - anschluss kosten, d38_ke - spezifischer, g38_ke - quantität

sum_inv_kg400 = j34_ke + j35_ke + j38_ke
print('sum_inv_kg400 = ', sum_inv_kg400)


#Investitionskosten [€] - Windkraft

#calculate j44_ke - windkraft offshore kosten (Windkraft)
d44_ke = 3500       #d44_ke - spezifischer
g44_ke = c22_2ndlayer*1000      #INPUT!!!!!
inv_windkraft = d44_ke*g44_ke   #j44_ke - windkraft offshore kosten, d44_ke - spezifischer, g44_ke - quantität
print('inv_windkraft = ', inv_windkraft)


#Investitionskosten [€] - PV

#calculate j48_ke - PV total kosten (PV)
d48_ke = 1200       #d48_ke - spezifischer
if j2_input == 'Neubau':
	q15_flachen = i2_input*c20_2ndlayer		#INPUT!!!!!!
	q25_flachen = 0
else:
	q15_flachen = 0
	q25_flachen = i2_input*c20_2ndlayer		#INPUT!!!!!!
c8_solar = q25_flachen + q15_flachen		#c8_solar - total effective area solar (m2)
c5_solar = 0.16		#c5_solar - PV effizienz solar
c9_solar = c8_solar*c5_solar		#c9_solar - leistung solar (kW)
g48_ke = c9_solar      #g48_ke - mercedes PV preis (kWp)
inv_pv = d48_ke*g48_ke   #j48_ke - PV total kosten, d48_ke - spezifischer, g48_ke - quantität
print('inv_pv = ', inv_pv)

#Investitionskosten [€] - Batterie

#calculate j51_ke - batterie tesia powerpacks kosten
d51_ke = 55000       #d51_ke - spezifischer
j4_solar = c25_2ndlayer		#INPUT!!!!!!
g51_ke = round(j4_solar/210/0.3)*0.3      #g51_ke - powerpacks
inv_batterie = d51_ke*g51_ke   #j51_ke - batterie kosten, d51_ke - spezifischer, g51_ke - quantität
print('inv_batterie = ',inv_batterie)


#Investitionskosten [€] - Nahwaermespeicher

#calculate j55_ke - nahwarmespeicher kosten
d55_ke = 16000/10       #d55_ke - spezifischer
n4_nahwarmespeicher = c24_2ndlayer  #INPUT!!!!!
radius_tank = 1.5       #n5_nahwarmespeicher
hohe = 7    #n6_nahwarmespeicher
n7_nahwarmespeicher = hohe*pi*radius_tank**2
g55_ke = n4_nahwarmespeicher * n7_nahwarmespeicher      #g55_ke - (m3)
inv_nahwarmespeicher = d55_ke*g55_ke   #j55_ke - nahwarmespeicher kosten, d55_ke - spezifischer, g55_ke - quantität
print('inv_nahwarmespeicher =', inv_nahwarmespeicher)


#Investitionskosten [€] - Gesamt

#calculate investitionskosten gesamt
inv_gesamt = sum_inv_kg200 + sum_inv_kg400 + inv_windkraft + inv_pv + inv_batterie + inv_nahwarmespeicher
print('inv_gesamt = ',inv_gesamt)

#Graue THG Emission [tCO2e] - KG200

#calculate s8_ke - infrastruktur heizzentrale graue emissionen (kg200 - heizzentrale)

p8_ke = 1.53    #p8_ke - spezifische Graue Emissionen
n8_ke = n15_flachen + n25_flachen   #n8_ke - quantität
s8_ke = p8_ke*n8_ke/1000    #s8_ke - infrastruktur heizzentrale graue emissionen

#calculate s9_ke - infrastruktur EMSR graue emissionen (kg200 - heizzentrale)

p9_ke = 12.7    #p9_ke - spezifische Graue Emissionen
n9_ke = n8_ke   #n9_ke - quantität
s9_ke = p9_ke*n9_ke/1000    #s9_ke - infrastruktur EMSR graue emissionen

#calculate s10_ke - infrastruktur pufferspeicher graue emissionen (kg200 - heizzentrale)

p10_ke = 3.5    #p10_ke - spezifische Graue Emissionen
n10_ke = g10_ke*2000   #n10_ke - quantität
s10_ke = p10_ke*n10_ke/1000    #s10_ke - infrastruktur pufferspeicher graue emissionen

#calculate s12_ke - infrastruktur low-ex netwerk graue emissionen (kg200 - heizzentrale)

p12_ke = 1260    #p12_ke - spezifische Graue Emissionen
g12_ke = c8_2ndlayer    #INPUT!!!!!!
n12_ke = g12_ke   #n12_ke - quantität
s12_ke = p12_ke*n12_ke/1000    #s12_ke - infrastruktur low-ex netwerk graue emissionen

#calculate s20_ke - energiequelle BHKW graue emissionen (kg200 - heizzentrale)

p20_ke = 2180/8/2    #p20_ke - spezifische Graue Emissionen
n20_ke = g20_ke   #n20_ke - quantität
s20_ke = p20_ke*n20_ke/1000    #s20_ke - energiequelle BHKW graue emissionen

#calculate s22_ke - energiequelle abwasserwarme graue emissionen (kg220 - heizzentrale)

p22_ke = 2180/8    #p22_ke - spezifische Graue Emissionen
n22_ke = g22_ke   #n22_ke - quantität
s22_ke = p22_ke*n22_ke/1000    #s22_ke - energiequelle abwasserwarme graue emissionen


#calculate s27_ke - energiequelle solarthermie graue emissionen (kg270 - heizzentrale)

p27_ke = 155    #p27_ke - spezifische Graue Emissionen
j7_erneuerbare = (p25_flachen + p15_flachen)*c21_2ndlayer   #INPUT!!!!!
j8_erneuerbare = 1      #nutzungsfaktor solarthermie
n27_ke = j7_erneuerbare*j8_erneuerbare   #n27_ke - quantität
s27_ke = p27_ke*n27_ke/1000    #s27_ke - energiequelle solarthermie graue emissionen

#calculate s30_ke - energiequelle geothermie graue emissionen (kg300 - heizzentrale)

p30_ke = 28.1    #p30_ke - spezifische Graue Emissionen

if n8_ke == 0:
    n30_ke = 0			#n30_ke - quantität
else:
    n30_ke = c10_erneuerbare*c13_erneuerbare			#n30_ke - quantität  
s30_ke = p30_ke*n30_ke/1000    #s30_ke - energiequelle geothermie graue emissionen


#calculate s31_ke - energiequelle warmepumpe zentral graue emissionen

p31_ke = 2180/8		#p31_ke - spezifische Graue Emissionen
n31_ke = l50_syst + l60_syst + l70_syst + l40_syst			#n31_ke - quantität
s31_ke = p31_ke*n31_ke/1000    #s31_ke - energiequelle warmepumpe zentral graue emissionen

graue_thg_em_kg200 = s8_ke + s9_ke + s10_ke + s12_ke + s20_ke + s22_ke + s27_ke + s30_ke + s31_ke		#graue_thg_em_kg200 - Graue THG Emissionen [tCO2e] - KG200
print('graue_thg_em_kg200 = ', graue_thg_em_kg200)

#Graue THG Emissionen [tCO2e] - KG400-Technik

#calculate s34_ke - anhebung warmepumpen dezentral graue emissionen

p34_ke = 2910/8		#p34_ke - spezifische Graue Emissionen
n34_ke = (ae48_syst + ae57_syst + ae66_syst)/5000*1000		#n34_ke - quantität
s34_ke = p34_ke*n34_ke/1000			#s34_ke - anhebung warmepumpen dezentral

#calculate s35_ke - gebaude graue emissionen

p35_ke = 3.07 + 6.06		#p35_ke - spezifische Graue Emissionen
n35_ke = n15_flachen + n25_flachen 		#n35_ke - quantität		INPUT!!!!!
s35_ke = p35_ke*n35_ke/1000		#s35_ke - gebaude graue emissionen

graue_thg_em_kg400 = s34_ke + s35_ke		#graue_thg_em_kg400 - Graue THG Emissionen [tCO2e] - KG400-Technik
print('graue_thg_em_kg400 = ', graue_thg_em_kg400)

					
#Graue THG Emissionen [tCO2e] - Windkraft

p44_ke = 1600/3		#p44_ke - spezifische
n44_ke = c22_2ndlayer		#n44_ke - quantität c22_2ndlayer - INPUT!!!!!!
s44_ke = p44_ke*n44_ke			#s44_ke - graue emissionen - windkraft
graue_thg_em_wind = s44_ke
print('graue_thg_em_wind = ', graue_thg_em_wind)

#Graue THG Emissionen [tCO2e] - PV

p48_ke = 2080		#p48_ke - spezifische
n48_ke = c9_solar		#n48_ke - quantität
s48_ke = p48_ke*n48_ke/1000		#s48_ke - graue emissionen - PV total
graue_thg_em_pv = s48_ke
print('graue_thg_em_pv = ', graue_thg_em_pv)

#Graue THG Emissionen [tCO2e] - Batterie

p52_ke = 185 		#p52_ke - spezifische
n52_ke = c25_2ndlayer		#INPUT!!!!
s52_ke = p52_ke*n52_ke/1000			#s52_ke - graue emissionen batterie alternativ berechnung
p51_ke = 80.6 		#p51_ke - spezifische
n51_ke = c25_2ndlayer		#INPUT!!!!!
s51_ke = p51_ke*n51_ke/1000
if c26_2ndlayer == "Lithium":		#INPUT!!!!!
	graue_thg_em_bat = s52_ke		#s52_ke - graue emissionen batterie alternativ berechnung
else:
	if c26_2ndlayer == "Second-Life Lithium":			#INPUT!!!!!
		graue_thg_em_bat = s51_ke		#s51_ke - graue emissionen batterie tesia powerpacks
	else:
		graue_thg_em_bat = 0
		
#Graue THG Emissionen [tCO2e] - Nahwaermespeicher

p55_ke = 540/4*3.51		#p55_ke - spezifische, 3.51 - stahlblech verzinkt
n55_ke = n4_nahwarmespeicher*n7_nahwarmespeicher		#n55_ke - quantität
graue_thg_em_nahw = p55_ke*n55_ke/1000
print('graue_thg_em_nahw = ', graue_thg_em_nahw)

#Graue THG Emissionen [tCO2e] - Gesamt

#graue_thg_em_gesamt = graue_thg_em_kg200 + graue_thg_em_kg400 + graue_thg_em_kg300 + graue_thg_em_wind + graue_thg_em_pv + graue_thg_em_bat + graue_thg_em_nahw

graue_thg_em_gesamt = graue_thg_em_kg200 + graue_thg_em_kg400 + graue_thg_em_wind + graue_thg_em_pv + graue_thg_em_bat + graue_thg_em_nahw
print('graue_thg_em_gesamt = ', graue_thg_em_gesamt)


#create result csv

fieldnames = ['id','Investitionskosten - KG200', 'Investitionskosten - KG400-Technik', 'Investitionskosten - Windkraft', 'Investitionskosten - PV', 'Investitionskosten - Batterie', 'Investitionskosten - Nahwaermespeicher', 'Investitionskosten - Gesamt','Graue THG Emissionen - KG200', 'Graue Emissionen - KG400-Technik','Graue THG Emissionen - Windkraft','Graue THG Emissionen - PV','Graue THG Emissionen - Batterie','Graue THG Emissionen - Nahwaermespeicher','Graue THG Emissionen - Gesamt']

export_file = open('C:/Users/eusmanova/Downloads/LCA_tool_result.csv', 'w')
file = csv.DictWriter(export_file, fieldnames, delimiter=';',quotechar='"', lineterminator = '\n', quoting=csv.QUOTE_ALL)

headers = {} 
for n in fieldnames:
    headers[n] = str(n)

file.writerow(headers)

row = dict()
#print(adresses[addrs])
row['id'] = '20430'
row['Investitionskosten - KG200'] = str(round(sum_inv_kg200,3)).replace('.', ',')
row['Investitionskosten - KG400-Technik'] = str(round(sum_inv_kg400,3)).replace('.', ',')
row['Investitionskosten - Windkraft'] = str(round(inv_windkraft,3)).replace('.', ',')
row['Investitionskosten - PV'] = str(round(inv_pv,3)).replace('.', ',')
row['Investitionskosten - Batterie'] = str(round(inv_batterie,3)).replace('.', ',')
row['Investitionskosten - Nahwaermespeicher'] = str(round(inv_nahwarmespeicher,3)).replace('.', ',')
row['Investitionskosten - Gesamt'] = str(round(inv_gesamt,3)).replace('.', ',')
row['Graue THG Emissionen - KG200'] = str(round(graue_thg_em_kg200,3)).replace('.', ',')
row['Graue Emissionen - KG400-Technik'] = str(round(graue_thg_em_kg400,3)).replace('.', ',')
row['Graue THG Emissionen - Windkraft'] = str(round(graue_thg_em_wind,3)).replace('.', ',')
row['Graue THG Emissionen - PV'] = str(round(graue_thg_em_pv,3)).replace('.', ',')
row['Graue THG Emissionen - Batterie'] = str(round(graue_thg_em_bat,3)).replace('.', ',')
row['Graue THG Emissionen - Nahwaermespeicher'] = str(round(graue_thg_em_nahw,3)).replace('.', ',')
row['Graue THG Emissionen - Gesamt'] = str(round(graue_thg_em_gesamt,3)).replace('.', ',')
file.writerow(row)
export_file.close()

