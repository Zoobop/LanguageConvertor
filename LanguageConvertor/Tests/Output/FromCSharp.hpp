#pragma once

#include "Abstraction.h"

namespace Output
{
    class FromCSharp : public Base, public IInterface
    {
        public int32_t number = 0;
        public float weight;
        private std::string stringPropertyBackingField;
        private int32_t intPropertyBackingField;
        private IAnimal animalBackingField;
        
        public void method()
        {
            
        }
        
        public static void findAnimal(IAnimal animal)
        {
            
        }
        
        public std::string getStringProperty()
        {
                return stringPropertyBackingField;
        }
        
        public void setStringProperty(std::string value)
        {
                stringPropertyBackingField = value;
        }
        
        public int32_t getIntProperty()
        {
                return intPropertyBackingField;
        }
        
        public IAnimal getAnimal()
        {
                return animalBackingField;
        }
        
        private static int32_t add(int[] numbers,int32_t count)
        {
            var sum = 0;
            for (var i = 0; i < count; i++)
            {
                sum += numbers[i];
            }
            return sum;
        }
        
        void explode(bool sure)
        {
            
        }
        
        private void setAnimal(IAnimal value)
        {
                animalBackingField = value;
        }
        
        protected static void func(std::string obj)
        {
            
        }
        
    }
    
}
