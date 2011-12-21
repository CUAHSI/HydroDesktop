
soubor = "moskva01.dat"
Nazevstanice = "Moskva"

limitniS = 10   # od jakeho S se pocita, ze je sucho


nastavsucho = 10  # obrazek od jakeho S 

nastavsucho1 = 1  # obrazek od jakeho S 
nastavsucho2 = 10  # obrazek od jakeho S 
nastavsucho3 = 20  # obrazek od jakeho S 
nastavsucho4 = 50  # obrazek od jakeho S 
nastavsucho5 = 100  # obrazek od jakeho S 

nastavsucho6 = 9999  # obrazek od jakeho S

#JIRKA - nastaveni promennych z prikazove radky
args <- commandArgs(trailingOnly = TRUE) #parametry prikazove radky skriptu
if (length(args) > 0)
	setwd(args[1]) #prvni parametr je slozka, kam se ulozi vystupni soubory
if (length(args) > 1)
	soubor= args[2] #druhy parametr je nazev vstupniho souboru
if (length(args) > 2)
	Nazevstanice = args[3] #treti parametr je nazev stanice
#KONEC JIRKA - nastaveni promennych z prikazove radky 

data = read.table(soubor, header=TRUE)

sb = Nazevstanice

sra = data$Srazky
tep = data$Teploty
dni = length(sra)
zapornet = vector("logical", dni)
kalendar = data$Datum

datumkalendar = as.Date(as.character(kalendar), "%Y-%m-%d")

prvniclen = datumkalendar[1]
posledniclen = datumkalendar[length(datumkalendar)]
radadatumu =  seq(as.Date(paste(prvniclen, sep='')), as.Date( paste(posledniclen, sep='')), by="day")
dnirada = length(radadatumu)
dnikalendar = length(datumkalendar)
shoda = radadatumu%in%datumkalendar

den =format(datumkalendar, "%d")
mesic =format(datumkalendar, "%m")
rok =format(datumkalendar, "%Y")
rok1 = as.integer(rok)
mesic1 = as.integer(mesic)
den1 = as.integer(den)


for (d in 1:dni)
  zapornet[d] = ifelse(tep[d] < 0, TRUE, FALSE)

z = vector("numeric", dni)
for (d in 1:dni) {
  z[d] = -1
  horni_limit = 1e-5
  for (i in 0:20) {
    if (sra[d] < horni_limit)
      break
    else {
      horni_limit = horni_limit + 0.2 * 2 ^ i
      z[d] = z[d] + 1
    }
  }
}
SR = cumsum(z)

sucho_zac = NULL
sucho_kon = NULL
posl_zpracovany = 0
konec = FALSE

while (!konec) {
  #minimum, vÅ¡echny pos absolutnÄ› k celÃ© SR
  SR_min_pos = posl_zpracovany + which.min(SR[(posl_zpracovany + 1):dni]) #relativnÃ­!
  min_nalez = FALSE
  while (!min_nalez) {
    SR_min_dalsi_pos = SR_min_pos + which.min(SR[(SR_min_pos + 1):dni])
    if (SR[SR_min_dalsi_pos] - SR[SR_min_pos] > 1e-5)
      min_nalez = TRUE
    else  #prvnÃ­ minimum -> vÅ¾dy platÃ­ (SR_min_dalsi_pos > SR_min_pos)
      SR_min_pos = SR_min_dalsi_pos
  }
  sucho_kon = c(sucho_kon, SR_min_pos)

  #pÅ™edchozÃ­ maximum
  SR_max_pos = posl_zpracovany + which.max(SR[(posl_zpracovany + 1):(SR_min_pos - 1)]) #z definice which.max vÅ¾dy prvnÃ­
  sucho_zac = c(sucho_zac, SR_max_pos + 1)

    if (sucho_kon[1] < sucho_zac[1]) sucho_kon[1] = sucho_zac

  #odkud se bude brÃ¡t zbytek
  vzestup = TRUE
  pos = SR_min_pos
  while (vzestup) {
    if (SR[pos + 1] < SR[pos] - 1e-5) {
      posl_zpracovany = pos - 1
      vzestup = FALSE
    }
    else
      pos = pos + 1
    if (pos == dni) {
      vzestup = FALSE
      konec = TRUE
    }
  }
}

such = length(sucho_zac)
T_suma = vector("numeric", such)
T_nezap = vector("numeric", such)
S_index = vector("numeric", such)
SRA_suma = vector("numeric", such)
Z_rozdil = vector("numeric", such)
trvani = vector("numeric", such)
sucho = rep("NE", dni)

for (su in 1:such) {
  tmp_obd = sucho_zac[su]:sucho_kon[su]
  sucho[tmp_obd] = "ANO"
  tmp_tep = tep[tmp_obd]
  tmp_sra = sra[tmp_obd]
  T_suma[su] = sum(tmp_tep)
  tmp_T_nezap = tapply(tmp_tep, zapornet[tmp_obd], sum)
  if (length(tmp_T_nezap) == 1 && names(tmp_T_nezap)[1] == "TRUE") #jen zÃ¡pornÃ©
    T_nezap[su] = 0
  else  
    T_nezap[su] = tmp_T_nezap[["FALSE"]]
  SRA_suma[su] = sum(tmp_sra)
  Z_rozdil[su] = SR[sucho_zac[su] - 1] - SR[sucho_kon[su]]
  S_index[su] = Z_rozdil[su] / 1000 * T_nezap[su]
  trvani[su] =  sucho_kon[su] - sucho_zac[su] + 1
}


sucho_df = data.frame(NA, dim = c(such,8))
trid_trvani = data.frame(NA, dim = c(such,8))
trid_sindex = data.frame(NA, dim = c(such,8))

for (su in 1:such )
{
sucho_df[su,1] = kalendar[sucho_zac[su]]
sucho_df[su,2] = kalendar[sucho_kon[su]] 
sucho_df[su,3] =  trvani[su]
sucho_df[su,4] =  S_index[su]
sucho_df[su,5] = T_suma[su]
sucho_df[su,6] =  T_nezap[su] 
sucho_df[su,7] = SRA_suma[su]
sucho_df[su,8] =  Z_rozdil[su]
}


trid_trvani = sucho_df[sort(sucho_df[,3] , decreasing=TRUE, ind=T)$ix,]
trid_sindex = sucho_df[sort(sucho_df[,4] , decreasing=TRUE, ind=T)$ix,]

# do rady
S_index_rada = rep(0, dni)
T_suma_rada = rep(0, dni)
T_nezap_rada = rep(0, dni)
SRA_suma_rada = rep(0, dni)
Z_rozdil_rada = rep(0, dni)
trvani_rada = vector("numeric", such)

for (ob1 in 1:such )
{
pom_radadatumu = seq(datumkalendar[sucho_zac[ob1]],datumkalendar[sucho_kon[ob1]],by="day")
S_index_rada[radadatumu%in%pom_radadatumu]= S_index[ob1]
T_suma_rada[radadatumu%in%pom_radadatumu]= T_suma[ob1]
T_nezap_rada[radadatumu%in%pom_radadatumu]= T_nezap[ob1]
SRA_suma_rada[radadatumu%in%pom_radadatumu]= SRA_suma[ob1]
Z_rozdil_rada[radadatumu%in%pom_radadatumu]= Z_rozdil[ob1]
trvani_rada[radadatumu%in%pom_radadatumu] = trvani[ob1]
}





prvni_rok = as.integer(format(prvniclen , "%Y")) 
posledni_rok = as.integer(format(posledniclen , "%Y")) 
rozdil_let = posledni_rok - prvni_rok  + 1


if ( (prvni_rok %% 4 ) == 0 ) prest_let = ( rozdil_let %/% 4 ) + 1  else prest_let = ( rozdil_let %/% 4 )




if (prvniclen == as.Date(paste(rok[1],'-01-01',sep='')) ) 
{

prvnihgrafua = prvniclen
prvnivektora = c()
prvnihodnotya = c()
rok1a = c()
mesic1a = c()
den1a = c()

} else 
{
prvnihgrafua = as.Date(paste(rok[1],'-01-01',sep=''))
poslednihgrafua = prvniclen - 1
prvnivektora = seq(as.Date(prvnihgrafua), as.Date(poslednihgrafua), by="day")
delkaprvnia = length(prvnivektora)
prvnihodnotya = rep(9999,delkaprvnia )

dena =format(prvnivektora, "%d")
mesica =format(prvnivektora, "%m")
roka =format(prvnivektora, "%Y")
rok1a = as.integer(roka)
mesic1a = as.integer(mesica)
den1a = as.integer(dena)

}


if ( posledniclen == as.Date(paste(rok[dnirada],'-12-31',sep='')) )
{
poslednihgrafu = posledniclen
poslednivektor = c()
poslednihodnoty = c()
rok1b = c()
mesic1b = c()
den1b = c()

} else
{
poslednihgrafu = as.Date(paste(rok[dnirada],'-12-31',sep=''))
poslednivektor = seq(as.Date(posledniclen+1), as.Date( poslednihgrafu ), by="day") 
delkaposledni = length(poslednivektor )
poslednihodnoty  = rep(9999,delkaposledni  )

denb =format(poslednivektor, "%d")
mesicb =format(poslednivektor, "%m")
rokb =format(poslednivektor, "%Y")
rok1b = as.integer(rokb)
mesic1b = as.integer(mesicb)
den1b = as.integer(denb)


}



obdobigrafu = seq(as.Date(prvnihgrafua), as.Date( poslednihgrafu), by="day") 
datagrafu = c(prvnihodnotya, S_index_rada , poslednihodnoty)
delkagrafu = length(datagrafu )

dencelek = c(den1a, den1, den1b)
mesiccelek = c(mesic1a, mesic1, mesic1b)
rokcelek = c(rok1a, rok1, rok1b)




if ( prvni_rok < 1900 ) 
{
delkaneprestrady = delkagrafu - prest_let + 1
delkaprestlet =  prest_let - 1
} else {
delkaneprestrady = delkagrafu - prest_let 
delkaprestlet =  prest_let 
}




# Hodnoceni podle velikosti suchosti indexu S 

krit1 = vector("numeric",delkagrafu )
krit2 = vector("numeric",delkagrafu )

for (d in 1: dnirada  )
{
if ( S_index_rada[d] > nastavsucho ) krit1[d] = 0 else krit1[d] = 1
}



for (d in 1: delkagrafu  )
{
if ( datagrafu[d] == nastavsucho6 ) 
krit2[d] = 0
else if ( datagrafu[d] > nastavsucho5 ) 
krit2[d] = 1 
else if ( datagrafu[d] > nastavsucho4 ) 
krit2[d] = 2 
else if ( datagrafu[d] > nastavsucho3 ) 
krit2[d] = 3 
else if ( datagrafu[d] > nastavsucho2 ) 
krit2[d] = 4
else if ( datagrafu[d] > nastavsucho1 ) 
krit2[d] = 5
else 
krit2[d] = 6
}












den_neprestupny =  vector("numeric",delkaneprestrady )
mesic_neprestupny = vector("numeric",delkaneprestrady ) 
rok_neprestupny =  vector("numeric",delkaneprestrady )
sucho_neprestupny = vector("numeric",delkaneprestrady )
krit1_neprestupny = vector("numeric",delkaneprestrady )
krit2_neprestupny = vector("numeric",delkaneprestrady )

den_prestupny =  vector("numeric",delkaprestlet )
mesic_prestupny = vector("numeric", delkaprestlet) 
rok_prestupny =  vector("numeric",delkaprestlet )
sucho_prestupny = vector("numeric",delkaprestlet )

prestupded_poradi = vector("numeric", delkaprestlet  )



mp = 1
mn = 1
prestupded_poradi = NULL 

for (d in 1: delkagrafu   )
{
if ((( mesiccelek[d] == 2) & ( dencelek[d] == 29)))
{
den_prestupny[mn]= den1[d]
mesic_prestupny[mn] = mesic1[d]
rok_prestupny[mn] = rok1[d]
sucho_prestupny[mn] = datagrafu[d]
prestupded_poradi = c(prestupded_poradi,d)
mn = mn + 1
} else
{
den_neprestupny[mp]= den1[d]
mesic_neprestupny[mp] = mesic1[d]
rok_neprestupny[mp] = rok1[d]
sucho_neprestupny[mp] = datagrafu[d]
krit1_neprestupny[mp] = krit1[d]
krit2_neprestupny[mp] = krit2[d]
mp = mp + 1
}
}

delkaneprestrady = length(sucho_neprestupny)







sink("sucha-rada.txt")
cat(paste("Od", "Trvani", "Je sucho" , "S index", "Suma T", "Suma T nezap", "Suma SRA", "Rozdil Z", sep="\t"))  
cat("\n")
for (su in 1:dni) {
 cat(paste(datumkalendar[su] , sucho[su],trvani_rada[su] , S_index_rada[su], T_suma_rada[su], T_nezap_rada[su], SRA_suma_rada[su], Z_rozdil_rada[su], sep="\t"))
 cat("\n")
}
sink()

sink("sucha.txt")
cat(paste("Od", "Do", "Trvani", "S index", "Suma T", "Suma T nezap", "Suma SRA", "Rozdil Z", sep="\t"))  
cat("\n")
for (su in 1:such) {
 cat(paste(datumkalendar[sucho_zac[su]], datumkalendar[sucho_kon[su]], trvani[su], S_index[su], T_suma[su], T_nezap[su], SRA_suma[su], Z_rozdil[su], sep="\t"))
 cat("\n")
}
sink()


sink("sucha_trvani.txt")
cat(paste("Od", "Do", "Podle trvani", "S index", "Suma T", "Suma T nezap", "Suma SRA", "Rozdil Z", sep="\t"))  
cat("\n")
for (su in 1:such) {
 cat(paste(datumkalendar[trid_trvani[su,1]],datumkalendar[trid_trvani[su,2]],trid_trvani[su,3],trid_trvani[su,4],trid_trvani[su,5],trid_trvani[su,6],trid_trvani[su,7], trid_trvani[su,8], sep="\t"))
 cat("\n")
}
sink()


sink("sucha_S_index.txt")
cat(paste("Od", "Do", "Trvani", "Podle S indexu", "Suma T", "Suma T nezap", "Suma SRA", "Rozdil Z", sep="\t"))  
cat("\n")
for (su in 1:such) {
 cat(paste(datumkalendar[trid_sindex[su,1]],datumkalendar[trid_sindex[su,2]],trid_sindex[su,3],trid_sindex[su,4],trid_sindex[su,5],trid_sindex[su,6],trid_sindex[su,7], trid_sindex[su,8], sep="\t"))
 cat("\n")
}
sink()



sink("chybejicidata.txt")
cat(paste("V souboru", soubor , "chybi data z nasledujicich dnu: "))
cat("\n")
for (i in 1:dni)
{
if (shoda[i] == FALSE) 
{
cat(paste(radadatumu[i]))
cat("\n")
}
}
sink()



if  (rozdil_let > 100 ) 
{
rokmrizka = 10
}else if  (rozdil_let > 50 ) 
{
rokmrizka = 5
}else if  (rozdil_let > 15 ) 
{
rokmrizka = 3
} else
{
rokmrizka = 1
}

velmat = delkaneprestrady  %/% 365
mat_sucho<-matrix(sucho_neprestupny,365, velmat)
mat_sucho_transf = t(mat_sucho)

mat_sucho2<-matrix(krit1_neprestupny,365, velmat)
mat_sucho2_transf = t(mat_sucho2<-matrix(krit1_neprestupny,365, velmat))

mat_sucho3<-matrix(krit2_neprestupny,365,velmat)
mat_sucho3_transf = t(mat_sucho3)





druhy = prvni_rok+velmat
prvnileden  <- structure(0 ,class="Date")
poslpros  <- structure(364 ,class="Date")

irr = 1
jedenacty_rok = prvni_rok+9
for ( ppdd in prvni_rok:jedenacty_rok)
{
if (ppdd %% 10  == 0) prok = irr 
irr = irr + 1 
}
prvni_rok_graf = prvni_rok + prok - 1 
prvni_rok_graf = prvni_rok + rokmrizka


pocet_hodnot_graf = ((druhy - prvni_rok_graf) %% 10 ) + 1


grok=  vector("numeric",pocet_hodnot_graf )
grok = NULL
grok =  prvni_rok
for ( ppd in (prvni_rok_graf ):druhy)
{
if (ppd %% rokmrizka  == 0) grok = c(grok,ppd) 
}

dnyrokosa =  c(1 , 32 , 60 , 91 , 121 , 152 , 182 , 213 , 244 , 274 , 305 , 335)
dnyrokunazev = c("1.1.","1.2.","1.3.","1.4.","1.5.","1.6.","1.7.","1.8.","1.9.","1.10.","1.11.","1.12.")
 

library(grid) 
cols <- colorRamp(c("black","black", "red", "orange" ,"yellow", "green") )









rok_hodn = NULL

for (rrr in 1: dni )        # vypis roku
{
if (( den1[rrr] == 1 ) &  ( mesic1[rrr] == 1 )) rok_hodn = c(rok_hodn, rok1[rrr] )
}

pocetlet_original = length(rok_hodn)
pocetlet = pocetlet_original - 1
prvni_rok = rok1[1]
posledni_rok = rok1[dnikalendar]
osaroky = prvni_rok:posledni_rok  

rozdil = prvni_rok %% rokmrizka 
prvni_rok_grafu = prvni_rok + rokmrizka - rozdil
rozdil2 = posledni_rok %% rokmrizka
posledni_rok_grafu = posledni_rok + rokmrizka -rozdil2


rozdil = prvni_rok %% rokmrizka 
prvni_rok_grafu = prvni_rok + rokmrizka - rozdil
rozdil2 = posledni_rok %% rokmrizka
posledni_rok_grafu = posledni_rok + rokmrizka - rozdil2
rokrada = seq(prvni_rok_grafu,posledni_rok,by=rokmrizka)   
rokrada = c(prvni_rok,rokrada, posledni_rok_grafu )
drokrada = as.Date(paste(rokrada,'-01-01',sep=''))

maxpr = max(S_index_rada)

if  ( maxpr > 50 )
{
osay = seq(0,signif(maxpr,1),by=10) 
} else if ( maxpr > 20 )
{
osay = seq(0,signif(maxpr,1),by=5) 
} else if ( maxpr > 10 )
{
osay = seq(0,signif(maxpr,1),by=2) 
} else
{
osay = seq(0,signif(maxpr,1),by=1) 
} 


jpeg(filename = paste(sb, "_S_index.jpg"), width = 1500, height = 1000, pointsize = 25, quality = 100, bg = "white", res = NA)
par(xaxs = "i", yaxs = "i")
plot( datumkalendar , S_index_rada , type = "h" , axes = F, sub='', col="red", xlab='Rok', ylab='Index suchosti S')
abline(v=drokrada, col='black')
abline(h=osay,col='black')
axis(1,at=drokrada,labels=rokrada, lty=1,lwd=1) 
axis(2,at=osay,labels=osay, las=0 ,lwd=1)
box(lty=1,lwd=1, col = 'black') 
   dev.off()





   


if  ( maxpr > nastavsucho5 )
{
 vypln1 =  c("black", "red", "orange" ,"yellow","ivory2","white")  
} else if  ( maxpr > nastavsucho4 )
{
 vypln1 =  c( "red", "orange" ,"yellow","ivory2","white") 
} else if  ( maxpr > nastavsucho3 ) 
{
vypln1 =  c( "orange" ,"yellow","ivory2","white") 
} else if ( maxpr > nastavsucho2 ) 
 {
 vypln1 =  c("yellow","ivory2","white") 
} else if  ( maxpr > nastavsucho1 ) 
 {
 vypln1 =  c("ivory2","white") 
} else 
{
vypln1 =  c("white") 
}


 if ((length(prvnivektora) == 0)  & (length(poslednivektor) == 0 ))
  { 

jpeg(filename = paste(sb,"_sucha_intenzita.jpg"), width = 1500, height = 1000, pointsize = 25, quality = 100, bg = "white", res = NA )

layout(matrix(c(1,2), nrow = 2) ,heights = c(8, 2))
par(mar = c(1, 4, 1, 1))

image(prvni_rok:druhy,1:365,mat_sucho3_transf, axes = F, col=vypln1, lwd=1  ,xlab="Rok",ylab="Den" )
abline(v=grok,lty=1,col='black')
abline(h=dnyrokosa,lty=1,col='black')
axis(1,at=grok,labels=grok,pos=1, lty=1,lwd=1) 
axis(2,at=dnyrokosa,labels=dnyrokunazev, las=1) 
box(lty=1,lwd=1, col = 'black')
par(mar = c(1, 1, 1, 1))
image(prvni_rok:druhy,1:365,mat_sucho3_transf, axes = F, col=c("white", "white", "white" ,"white","white","white"), lwd=1  ,xlab="",ylab="", frame.plot = FALSE, useRaster = FALSE, add = FALSE )


legend("bottom", title="Sucho", c("Malé","Støednì velké","Velké","Velmi velké","Extrémnì velké"), fill = c('ivory2', 'yellow', 'orange', 'red', 'black')  , horiz=TRUE)


dev.off()

} else 
{


jpeg(filename = paste(sb,"_sucha_intenzita.jpg"), width = 1500, height = 1000, pointsize = 25, quality = 100, bg = "white", res = NA )

layout(matrix(c(1,2), nrow = 2) ,heights = c(8, 2))
par(mar = c(1, 4, 1, 1))

image(prvni_rok:druhy,1:365,mat_sucho3_transf, axes = F, col=c("blue",vypln1), lwd=1  ,xlab="Rok",ylab="Den" )
abline(v=grok,lty=1,col='black')
abline(h=dnyrokosa,lty=1,col='black')
axis(1,at=grok,labels=grok,pos=1, lty=1,lwd=1) 
axis(2,at=dnyrokosa,labels=dnyrokunazev, las=1) 
box(lty=1,lwd=1, col = 'black')
par(mar = c(1, 1, 1, 1))
image(prvni_rok:druhy,1:365,mat_sucho3_transf, axes = F, col=c("white","white", "white", "white" ,"white","white","white"), lwd=1  ,xlab="",ylab="", frame.plot = FALSE, useRaster = FALSE, add = FALSE )


legend("bottom", title="Sucho", c("Malé","Støednì velké","Velké","Velmi velké","Extrémnì velké","Chybìjící data"), fill = c('ivory2', 'yellow', 'orange', 'red', 'black', 'blue')  , horiz=TRUE)


dev.off()



}










rozdildnu = dnirada - dnikalendar 









cat(paste("V obdobi od ", prvniclen ," do " ,posledniclen , "chybi data z ", rozdildnu, "dnu "))










cat("\n")


