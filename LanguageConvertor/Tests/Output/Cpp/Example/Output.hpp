#pragma once

#include <string>

namespace Output
{
    class Base
    {
        protected:
            virtual void func1(int32_t num)
            {

            }

            virtual void func2(std::string string)
            {
                
            }
    };

    class IInterface
    {
        public:
            virtual void func3(uint32_t obj) = 0;
    };
}