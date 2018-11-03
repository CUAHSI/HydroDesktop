import Muskingum
import networkx as nx
import time
import math
from StringIO import StringIO
from lxml import etree
from networkx import * 
import matplotlib.pyplot as plot
from sys import stdout
import ironclad

class Reach:
    def __init__(self, uid, From, To, travel_time, x):
        self.uid = uid
        self.K = float(travel_time)
        self.X = float(x)
        self.Inflow = {}                                                        
        self.Outflow = {}                                                      
        self.C1 = 0
        self.C2 = 0
        self.C3 = 0
        self.From = From
        self.To = To

class Engine:

    def __init__(self):
        # Create and output file for the reach flows
        f = open('../MuskingumRouting_reachFlows.csv','w')
        f.close()
        print("Service started.")

    #def initialize(self, configfile, network_relat_path, stream_att_path):
    def initialize(self, elementset):
        stdout.write("\n--- Muskingum Web Service: Initialize Begin \n");

        #---- Initalize global variables ----
        self.timestep_sec = 0 #time_step
        self.current_time_sec = 0 # current_time
        self.G = nx.DiGraph()       
        self.reaches = {} #Create reaches dictionary
        
        #---- Parse FROM, TO K, and X from the XML stream ----
        stdout.write("    Parsing FROM, K, and X from XML stream ... ");
        elements = StringIO(elementset)
        xmltree = etree.parse(elements)
        #loop through all of the elements within the elementset
        
        for element in xmltree.getiterator('element'):
            #loop through all throughhe child nodes of each element
            for child in element.iterdescendants():
                if child.tag == "From":
                    From = child.text
                elif child.tag =="To":
                    To = child.text
                elif child.tag == "K":
                    K = child.text
                elif child.tag == "X":
                    X = child.text
            uid = int(From)
            self.reaches[From] = Muskingum.Reach(uid, From, To, K, X)
           # self.G.add_edge(self.reaches[From], self.reaches[To])
            uid += 1
        stdout.write("done.\n");
        
        #---- Parse TIMESTEP, START TIME, and END TIME from the XML stream ----
        stdout.write("    Parsing start time, end time, and time step from XML stream ... ");
        time_format1 = "%m/%d/%Y %H:%M:%S"
        time_format2 = "%m/%d/%Y %H:%M:%S %p"
        for element in xmltree.getiterator('TimeHorizon'):
            for child in element.iterdescendants():
                if child.tag == "TimeStepInSeconds":
                    self.timestep_sec = float(child.text)
                elif child.tag == "StartDateTime":
                    try: #timeformat 1 assumes no AM/PM
                        start_datetime = time.mktime(time.strptime(child.text, time_format1))
                    except:  #timeformat 2 assumes AM/PM
                        start_datetime = time.mktime(time.strptime(child.text, time_format2))  
                elif child.tag == "EndDateTime":
                    try: #timeformat 1 assumes no AM/PM
                        end_datetime = time.mktime(time.strptime(child.text, time_format1))
                    except:  #timeformat 2 assumes AM/PM
                        end_datetime = time.mktime(time.strptime(child.text, time_format2))
        stdout.write("done.\n");
        
        #---- Add edges to DiGraph ----
        stdout.write("    Building graph ... ");
        for i in self.reaches:
            try:
                self.G.add_edge(self.reaches[i], self.reaches[self.reaches[i].To])
            except:
                pass
        self.n =  nx.topological_sort(self.G)
        stdout.write("done.\n");

        #write header for output file
        writer = open('../MuskingumRouting_reachFlows.csv','w')
        for i in self.n:
            writer.write('reach: '+ str(i.uid) + ',')
        writer.write('\n')
        
	
        #---- Calculate C1, C2, C3 from K and X ----
        stdout.write("    Calculating C1, C2, and C3 for reaches in graph ... ");
        for i in self.n:
            k = self.reaches[i.From].K
            x = self.reaches[i.From].X
            self.reaches[i.From].C1 = (self.timestep_sec /3600.0 - 2*k*x)/ (2*k*(1-x) + self.timestep_sec /3600.0)
            self.reaches[i.From].C2 = (self.timestep_sec /3600.0 + 2*k*x)/ (2*k*(1-x) + self.timestep_sec /3600.0)
            self.reaches[i.From].C3 = (2*k*(1-x) - self.timestep_sec /3600.0) / (2*k*(1-x) + self.timestep_sec /3600.0)

            c1 = self.reaches[i.From].C1
            c2 = self.reaches[i.From].C2
            c3 = self.reaches[i.From].C3
            
            #initialize Inflow and Outflow dictionaries for reach to Zero. Key is time in seconds
            t = 0
            while t <= (end_datetime - start_datetime):
                self.reaches[i.From].Inflow["%f" % t] = 0.0
                self.reaches[i.From].Outflow["%f" % t] = 0.0
                t += self.timestep_sec   # convert timstep in hrs to timestep in seconds
            
        stdout.write("done.\n");

        stdout.write("--- Muskingum Web Service: Initialize End \n\n");


        
        return True

    def performTimeStep(self, inflow_stream):
        stdout.write("\n--- Muskingum Web Service: Perform Time Step Begin --- \n");

        #Open the output file to append the calculated flows
        writer = open('../MuskingumRouting_reachFlows.csv','a')

        #---- advance current time by time step ----
        previous_time_sec = self.current_time_sec
        self.current_time_sec += self.timestep_sec
        stdout.write("    Previous time is %f seconds.\n" % previous_time_sec);
        stdout.write("    Current time is %f seconds.\n" %  self.current_time_sec);
              
        #---- parse XML stream to set inflow values for reaches ----
        stdout.write("    Get inflow values from XML stream ... ");
        elements = StringIO(inflow_stream)
        xmltree = etree.parse(elements)
        inflow = []
        for _inflow in xmltree.getiterator('inflow'):
            #loop through all the child nodes of each element
            for child in _inflow.iterdescendants():
                if child.tag == "reach":
                    inflow.append(float(child.text))
        stdout.write("done.\n")      
  
        #---- route flows for each reach using network topology----
        stdout.write("    --- Route flows --- \n");
        outflow = [] #Array that holds the outflows for all outlets, at the current timestep
        e = 0 #input element within the inflow array
        for i in self.n:
            stdout.write("        for reach " + str(i.uid) + "...");
             
            #extracting reach properties C1, C2, C3
            c1 = self.reaches[i.From].C1
            c2 = self.reaches[i.From].C2
            c3 = self.reaches[i.From].C3

            
            #get inflow from previous time step
            try:
                In1 = self.reaches[i.From].Inflow["%f" % previous_time_sec] #In1 = inflow from last timestep
            except:
                stdout.write ("PROBLEM GETTING PREVIOUS INFLOW FOR REACH " + str([i.From]) + "\n")
            #add inflow to In2
            try:
                #self.reaches[i.From].Inflow["%f" % self.current_time_sec] = inflow[e] #In2 = inflow from this timestep (inflow)
                #Infow for this timestep equals flow being passed to this reach plus any additional inflow from a runoff hydrograph
                
                #In2 = inflow from this timestep + 'inflow'
                self.reaches[i.From].Inflow["%f" % self.current_time_sec] += inflow[i.uid-1] 

                #make sure that the reach inflow is never negative, even when instablitiy occurs
                if self.reaches[i.From].Inflow["%f" % self.current_time_sec] < 0 :
                    self.reaches[i.From].Inflow["%f" % self.current_time_sec] = 0

            except:
                #There is no inflow from the watershed into the reach
                self.reaches[i.From].Inflow["%f" % self.current_time_sec] = 0.0
                stdout.write ("PROBLEM SETTING CURRENT INFLOW FOR REACH (Inflow = 0) \n")
                
            #get inflow from current time step
            In2 = self.reaches[i.From].Inflow["%f" % self.current_time_sec]



            #get outflow from last timestep
            Out1 = self.reaches[i.From].Outflow[ "%f" % previous_time_sec]         

            #---- Check to see if flow is negative ----
            
            #Muskingum routed outflow
            f = c1*In2 + c2*In1 + c3*Out1
        
            #This loop check to see if the solution of the Muskingum equation is unstable (i.e. calculation of negative flows)
            if f < 0.0:
                stdout.write("Reach " + str(i.From) + " is unstable.  \n \t Flow of "+str(f)+" will be set to 0.0 \n")
                #f = 0.0
            
            #Write the calculated flow value to the output file
            writer.write(str(f))
            writer.write(',')

            #Set Outflow from this reach
            self.reaches[i.From].Outflow["%f" % self.current_time_sec] = f

            #Write Outflow to Console 
            stdout.write(str(self.reaches[i.From].Outflow["%f" % self.current_time_sec]))

            #add this outflow to downstream reach's inflow
            if self.G.successors(i):
                    #self.reaches[self.G.successors(i)[0].From].Inflow["%f" % self.current_time_sec] += self.reaches[i.From].Outflow["%f" % self.current_time_sec]
                    self.reaches[self.G.successors(i)[0].From].Inflow["%f" % self.current_time_sec] += f
            else:
                successor = "outlet"
                outflow.append(self.reaches[i.From].Outflow["%f" % self.current_time_sec])
                    
            #Set the current inflow for this reach, equal to the inflow from the previous timestep
            self.reaches[i.From].Inflow["%f" % previous_time_sec] = In2


            #Next inflow element
            e += 1
            stdout.write(" done.\n");

        #close the writer
        writer.write('\n')
        writer.close()
            
        #---- Return outflow at the outlets ----
        outflow_stream = "<outflow>"
        for q in outflow:
            outflow_stream += "<outlet>"+ str(q) +"</outlet>"
        outflow_stream += "</outflow>"

        stdout.write("--- Muskingum Web Service: Perform Time Step End --- \n\n");
        return outflow_stream
        
    def finalize(self):
        # todo: set object to null
        return True





