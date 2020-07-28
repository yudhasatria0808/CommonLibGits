using Common.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Common.Controller
{
    [Route("api/[controller]")]
    public abstract class BaseApiController<T, TModel, Tentity> : ControllerBase where T : class
    {
        protected ApiResponeModel globalRespone = new ApiResponeModel();
        protected List<object> ListEntities = new List<object>();

        #region Reflection Variable
        readonly ConstructorInfo magicConstructorService;
        readonly object magicClassObjectService;
        readonly ConstructorInfo magicConstructorModel;
        readonly object magicClassObjectModel;
        #endregion

        public BaseApiController()
        {
            magicConstructorService = typeof(T).GetConstructor(Type.EmptyTypes);
            magicClassObjectService = magicConstructorService.Invoke(new object[] { });

            magicConstructorModel = typeof(TModel).GetConstructor(Type.EmptyTypes);
            magicClassObjectModel = magicConstructorModel.Invoke(new object[] { });
        }

        [HttpGet("getbyidbase")]
        public virtual ApiResponeModel GetById(Guid id)
        {
            try
            {
                MethodInfo magicMethod = typeof(T).GetMethod("Get");
                var data = magicMethod.Invoke(magicClassObjectService, new object[] { id, null, false });
                globalRespone.IsValid = true;
                globalRespone.Data = data;
            }
            catch (Exception e)
            {
                globalRespone.IsValid = false;
                globalRespone.ErrorMsg = e.Message;
            }

            return globalRespone;
        }

        [HttpGet("getallbase")]
        public virtual ApiResponeModel GetAll()
        {
            try
            {
                MethodInfo magicMethod = typeof(T).GetMethod("GetAll");
                var data = magicMethod.Invoke(magicClassObjectService, new object[] { null, false });
                globalRespone.IsValid = true;
                globalRespone.Data = data;
            }
            catch (Exception e)
            {
                globalRespone.IsValid = false;
                globalRespone.ErrorMsg = e.Message;
            }

            return globalRespone;
        }

        [HttpPost("savejson")]
        public virtual ApiResponeModel SaveJson([FromBody]TModel model)
        {
            return Save(model);
        }

        [HttpPost("savefromdata")]
        public virtual ApiResponeModel SaveFormData(TModel model)
        {
            return Save(model);
        }

        [HttpPost("deletebase")]
        public virtual ApiResponeModel Delete(Guid id)
        {
            try
            {
                MethodInfo magicMethod = typeof(T).GetMethod("SoftDelete");
                var data = magicMethod.Invoke(magicClassObjectService, new object[] { id, User.Identity.Name, null, false });
                globalRespone.IsValid = true;
                globalRespone.Data = "success";
            }
            catch (Exception e)
            {
                globalRespone.IsValid = false;
                globalRespone.ErrorMsg = e.Message;
            }

            return globalRespone;
        }

        private ApiResponeModel Save(TModel model)
        {
            try
            {
                MethodInfo magicMethod = null;
                //Mapping Model To Entities
                model.GetType().GetMethod("MappingModelToEntities").Invoke(model, null);
                var dictEntities = model.GetType().GetField("dictionaryEntities");

                if ((int)model.GetType().GetProperty("id").GetValue(model, null) == 0)
                {
                    magicMethod = typeof(T).GetMethod("Insert", new Type[] { typeof(Dictionary<Type, object>), typeof(string), typeof(IDbTransaction), typeof(bool) });
                }
                else
                {
                    magicMethod = typeof(T).GetMethod("Update", new Type[] { typeof(Dictionary<string, object>), typeof(string), typeof(IDbTransaction), typeof(bool) });
                }

                magicMethod.Invoke(magicClassObjectService, new object[] { (Dictionary<Type, object>)dictEntities.GetValue(model), User.Identity.Name, null, false });

                globalRespone.IsValid = true;
                globalRespone.Data = "success";
            }
            catch (Exception e)
            {
                globalRespone.IsValid = false;
                globalRespone.ErrorMsg = e.Message;
            }

            return globalRespone;
        }
    }
}
