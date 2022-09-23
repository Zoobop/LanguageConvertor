#pragma once

#include "Example/Output.hpp"

namespace Output
{
    class FromCSharp : public Base, public IInterface
    {
        public:
            FromCSharp()
            {
                StringProperty = "NULL";
                IntProperty = 0;
            }
            
            FromCSharp(const std::string& str, const int32_t& integer)
            {
                StringProperty = str;
                IntProperty = integer;
            }
            
            void method()
            {
            }
            
            void func3(const uint32_t& obj)
            {
            }
            
            const std::string& getStringProperty()
            {
                return stringPropertyBackingField;
            }
            
            const int32_t& getIntProperty()
            {
                return intPropertyBackingField;
            }
            
        protected:
            void func1(const int32_t& obj) override
            {
            }
            
            void func2(const std::string& obj) override
            {
            }
            
            void setStringProperty(const std::string& value)
            {
                stringPropertyBackingField = value;
            }
            
        private:
            static int32_t add(const int32_t*& numbers, const int32_t& count)
            {
                return 0;
            }
            
            void explode(const bool& sure)
            {
            }
            
        public:
            int32_t number = 0;
            float weight;
            
        private:
            std::string stringPropertyBackingField;
            int32_t intPropertyBackingField;
            
    };
    
}
