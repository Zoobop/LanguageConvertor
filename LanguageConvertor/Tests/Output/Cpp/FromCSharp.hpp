#pragma once

#include "Example/Output.hpp"

namespace Output
{
    class FromCSharp : public Base, public IInterface
    {
        public:
            void method()
            {
                
            }
            
            void func3(uint32_t obj)
            {
                
            }
            
            std::string getStringProperty()
            {
                    return stringPropertyBackingField;
            }
            
            int32_t getIntProperty()
            {
                    return intPropertyBackingField;
            }
            
        protected:
            void func1(int32_t obj) override
            {
                
            }
            
            void func2(std::string obj) override
            {
                
            }
            
            void setStringProperty(std::string value)
            {
                    stringPropertyBackingField = value;
            }
            
        private:
            static int32_t add(int[] numbers, int32_t count)
            {
                return 0;
            }
            
            void explode(bool sure)
            {
                
            }
            
        public:
            int32_t number = 0;
            float weight;
            
        private:
            std::string stringPropertyBackingField;
            int32_t intPropertyBackingField;
            
    }
    
}
