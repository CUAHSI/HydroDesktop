import Muskingum
import xmlrpclib

#Implement Service Methods
s = xmlrpclib.ServerProxy('http://localhost:8000')

c = Muskingum.Engine()

stream = "<elementset><element><From>20414</From><To>20424</To><K>0.12</K><X>0.5</X></element><element><From>20416</From><To>20414</To><K>0.13</K><X>0.2</X></element><element><From>20418</From><To>20416</To><K>0.11</K><X>0.2</X></element><element><From>20422</From><To>20440</To><K>0.1</K><X>0.3</X></element><element><From>20424</From><To>20422</To><K>0.15</K><X>0.2</X></element><element><From>20426</From><To>20430</To><K>0.17</K><X>0.2</X></element><element><From>20428</From><To>20418</To><K>0.2</K><X>0.2</X></element><element><From>20430</From><To>20428</To><K>0.3</K><X>0.3</X></element><element><From>20432</From><To>20426</To><K>0.2</K><X>0.4</X></element><element><From>20434</From><To>20432</To><K>0.4</K><X>0.4</X></element><element><From>20436</From><To>20434</To><K>0.3</K><X>0.5</X></element><element><From>20438</From><To>20436</To><K>0.5</K><X>0.3</X></element><element><From>20440</From><To>20458</To><K>0.4</K><X>0.3</X></element><element><From>20458</From><To>21528</To><K>0.22</K><X>0.3</X></element><element><From>21528</From><To>22062</To><K>0.3</K><X>0.2</X></element><element><From>22062</From><To>22150</To><K>0.3</K><X>0.3</X></element><element><From>22150</From><To>22192</To><K>0.34</K><X>0.2</X></element><element><From>22192</From><To>24586</To><K>0.35</K><X>0.4</X></element><element><From>24582</From><To>52900</To><K>0.37</K><X>0.3</X></element><element><From>24584</From><To>24590</To><K>0.4</K><X>0.5</X></element><element><From>24586</From><To>24584</To><K>0.29</K><X>0.2</X></element><element><From>24588</From><To>24582</To><K>0.2</K><X>0.2</X></element><element><From>24590</From><To>24588</To><K>0.11</K><X>0.2</X></element><TimeHorizon><TimeStepInSeconds>3600</TimeStepInSeconds><StartDateTime>06/21/2008 1:00:00 AM</StartDateTime><EndDateTime>06/21/2008 2:00:00 AM</EndDateTime></TimeHorizon></elementset>"
#stream = "<elementset><element><From>20414</From><To>20424</To><K>0.12</K><X>0.5</X></element><element><From>20416</From><To>20414</To><K>0.13</K><X>0.2</X></element></elementset>"

c.initialize(stream)
##timestep
inflowVals = "<inflow><reach>10</reach><reach>7</reach></inflow>"
c.performTimeStep(inflowVals)
##timestep 2
inflowVals = [2000, 0]
#c.performTimeStep(inflowVals)
##timestep 3
#inflowVals = [4200, 0]
#c.performTimeStep(inflowVals)

