soubor = "meteo.dat"
Nazevstanice = "Tusimice"

limitniS = 10   # od jakeho S se pocita, ze je sucho


nastavsucho = 10  # obrazek od jakeho S 

nastavsucho1 = 1  # obrazek od jakeho S 
nastavsucho2 = 10  # obrazek od jakeho S 
nastavsucho3 = 20  # obrazek od jakeho S 
nastavsucho4 = 50  # obrazek od jakeho S 
nastavsucho5 = 100  # obrazek od jakeho S 

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
  #minimum, všechny pos absolutne k celé SR
  SR_min_pos = posl_zpracovany + which.min(SR[(posl_zpracovany + 1):dni]) #relativní!
  min_nalez = FALSE
  while (!min_nalez) {
    SR_min_dalsi_pos = SR_min_pos + which.min(SR[(SR_min_pos + 1):dni])
    
    print(paste('SR_min_pos',SR_min_pos))
    print(paste('SR_min_dalsi_pos',SR_min_dalsi_pos))

    if (SR[SR_min_dalsi_pos] - SR[SR_min_pos] > 1e-5) {
      min_nalez = TRUE
    }
    else  { #první minimum -> vždy platí (SR_min_dalsi_pos > SR_min_pos)
      SR_min_pos = SR_min_dalsi_pos
      print(paste('SR_min_pos',SR_min_pos))
      print(paste('SR_min_dalsi_pos',SR_min_dalsi_pos))
    }
  }
  sucho_kon = c(sucho_kon, SR_min_pos)

  #predchozí maximum
  SR_max_pos = posl_zpracovany + which.max(SR[(posl_zpracovany + 1):(SR_min_pos - 1)]) #z definice which.max vždy první
  sucho_zac = c(sucho_zac, SR_max_pos + 1)

    if (sucho_kon[1] < sucho_zac[1]) sucho_kon[1] = sucho_zac

  #odkud se bude brát zbytek
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
  if (length(tmp_T_nezap) == 1 && names(tmp_T_nezap)[1] == "TRUE") #jen záporné
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





# Hodnoceni podle velikosti suchosti indexu S 

krit1 = vector("numeric",dnirada )
krit2 = vector("numeric",dnirada )

for (d in 1: dnirada  )
{
if ( S_index_rada[d] > nastavsucho ) krit1[d] = 0 else krit1[d] = 1
}



for (d in 1: dnirada  )
{
if ( S_index_rada[d] > nastavsucho5 ) 
krit2[d] = 0 
else if ( S_index_rada[d] > nastavsucho4 ) 
krit2[d] = 1 
else if ( S_index_rada[d] > nastavsucho3 ) 
krit2[d] = 2 
else if ( S_index_rada[d] > nastavsucho2 ) 
krit2[d] = 3
else if ( S_index_rada[d] > nastavsucho1 ) 
krit2[d] = 4
else 
krit2[d] = 5
}






prvni_rok = as.integer(format(prvniclen , "%Y")) 
posledni_rok = as.integer(format(posledniclen , "%Y")) 

rozdil_let = posledni_rok - prvni_rok  + 1

if ( (prvni_rok %% 4 ) == 0 ) prest_let = ( rozdil_let %/% 4 ) + 1  else prest_let = ( rozdil_let %/% 4 )


if ( prvni_rok < 1900 ) 
{
delkaneprestrady = dnirada - prest_let + 1
delkaprestlet =  prest_let - 1
} else {
delkaneprestrady = dnirada - prest_let 
delkaprestlet =  prest_let 
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

for (d in 1: dnirada  )
{
if ((( mesic1[d] == 2) & ( den1[d] == 29)))
{
den_prestupny[mn]= den1[d]
mesic_prestupny[mn] = mesic1[d]
rok_prestupny[mn] = rok1[d]
sucho_prestupny[mn] = S_index_rada[d]
prestupded_poradi = c(prestupded_poradi,d)
mn = mn + 1
}
else
{
den_neprestupny[mp]= den1[d]
mesic_neprestupny[mp] = mesic1[d]
rok_neprestupny[mp] = rok1[d]
sucho_neprestupny[mp] = S_index_rada[d]
krit1_neprestupny[mp] = krit1[d]
krit2_neprestupny[mp] = krit2[d]
mp = mp + 1
}
}



velmat = delkaneprestrady  %/% 365
mat_sucho<-matrix(sucho_neprestupny,365, velmat)
mat_sucho_transf = t(mat_sucho)

mat_sucho2<-matrix(krit1_neprestupny,365, velmat)
mat_sucho2_transf = t(mat_sucho2<-matrix(krit1_neprestupny,365, velmat))

mat_sucho3<-matrix(krit2_neprestupny,365,velmat)
mat_sucho3_transf = t(mat_sucho3)

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

jpeg(filename = paste(sb, "_souctova_rada.jpg"), width = 1500, height = 1000, pointsize = 25, quality = 100, bg = "white", res = NA)
   plot( SR , type = "l", panel.first = grid(lty=1,lwd=1), main=paste('Souctová rada pro stanici' , Nazevstanice), sub='', col="blue", lwd=2, xlab='Den rady', ylab='Suma Z')
for (su in 1:length(sucho_zac)) 
  {
  lines(sucho_zac[su]:sucho_kon[su], SR[sucho_zac[su]:sucho_kon[su]], col = "red", lwd = 2)
  }  
dev.off()


jpeg(filename = paste(sb, "_S_index.jpg"), width = 1500, height = 1000, pointsize = 25, quality = 100, bg = "white", res = NA)
par(xaxs = "i", yaxs = "i")
   plot( datumkalendar , S_index_rada , type = "h", panel.first = grid(lty=1,lwd=1) ,sub='', col="red", xlab='Rok', ylab='Index suchosti S')
dev.off()


druhy = prvni_rok+velmat
prvnileden  <- structure(0 ,class="Date")
poslpros  <- structure(364 ,class="Date")

irr = 1
jedenacty_rok = prvni_rok+9
for ( ppdd in prvni_rok:jedenacty_rok)
{
if ((ppdd %% 10)  == 0) prok = irr 
irr = irr + 1 
}
prvni_rok_graf = prvni_rok + prok - 1 

pocet_hodnot_graf = ((druhy - prvni_rok_graf) %% 10 ) + 1


grok=  vector("numeric",pocet_hodnot_graf )
grok = NULL
grok =  prvni_rok
for ( ppd in (prvni_rok_graf ):druhy)
{
if (ppd %% 10  == 0) grok = c(grok,ppd) 
}

dnyrokosa =  c(1 , 32 , 60 , 91 , 121 , 152 , 182 , 213 , 244 , 274 , 305 , 335)
dnyrokunazev = c("1.1.","1.2.","1.3.","1.4.","1.5.","1.6.","1.7.","1.8.","1.9.","1.10.","1.11.","1.12.")
 


jpeg(filename = paste(sb,"_sucha_intenzita.jpg"), width = 1500, height = 1000, pointsize = 25, quality = 100, bg = "white", res = NA )

layout(matrix(c(1,2), nrow = 2) ,heights = c(8, 2))
par(mar = c(1, 4, 1, 1))

jpeg(filename = paste(sb,"_sucha_intenzita.jpg"), width = 1500, height = 1000, pointsize = 25, quality = 100, bg = "white", res = NA )
image(prvni_rok:druhy,1:365,mat_sucho3_transf, axes = F, col=c("black", "red", "orange" ,"yellow","ivory2","white"), lwd=2  ,xlab="Rok",ylab="Den", frame.plot = TRUE )
abline(v=grok,lty=1,col='black')
abline(h=dnyrokosa,lty=1,col='black')
axis(1,at=grok,labels=grok,pos=1, lty=1,lwd=1) 
axis(2,at=dnyrokosa,labels=dnyrokunazev, las=1) 
box(lty=1,lwd=1, col = 'black')


par(mar = c(1, 1, 1, 1))
image(prvni_rok:druhy,1:365,mat_sucho3_transf, axes = F, col=c("white", "white", "white" ,"white","white","white"), lwd=1  ,xlab="",ylab="", frame.plot = FALSE, useRaster = FALSE, add = FALSE )


legend("bottom", title="Sucho", c("Malé","Stredne velké","Velké","Velmi velké","Extrémne velké"), fill = c('ivory2', 'yellow', 'orange', 'red', 'black')  , horiz=TRUE)


dev.off()

