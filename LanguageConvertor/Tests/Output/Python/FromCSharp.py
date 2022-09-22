

from Output import *

@dataclass
class FromCSharp(Base, IInterface):
    number = 0
    weight: float
    stringPropertyBackingField: str
    intPropertyBackingField: int
    
    def fromCSharp(self) -> None:
        StringProperty = "NULL";
        IntProperty = 0;
    
    def fromCSharp(self, str: str, integer: int) -> None:
        StringProperty = str;
        IntProperty = integer;
    
    def method(self) -> None:
        pass
    
    def func3(self, obj: object) -> None:
        pass
    
    def getStringProperty(self) -> str:
        return stringPropertyBackingField
    
    def getIntProperty(self) -> int:
        return intPropertyBackingField
    
    def add(self, numbers: list[int], count: int) -> int:
        return 0;
    
    def explode(self, sure: bool) -> None:
        pass
    
    def func1(self, obj: int) -> None:
        pass
    
    def func2(self, obj: str) -> None:
        pass
    
    def setStringProperty(self, value: str) -> None:
        self.stringPropertyBackingField = value
    

