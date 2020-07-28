using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public abstract class AbstractModelValidator<T> : AbstractValidator<T> where T : class
    {

    }
}
