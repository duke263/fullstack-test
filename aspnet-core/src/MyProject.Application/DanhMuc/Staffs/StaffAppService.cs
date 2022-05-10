namespace MyProject.DanhMuc.Staffs
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Text;
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Abp.Domain.Repositories;
    using Abp.Linq.Extensions;
    using Abp.UI;
    using DbEntities;
    using Microsoft.EntityFrameworkCore;
    using MyProject.DanhMuc.Staffs;
    using MyProject.DanhMuc.Staffs.Stos;
    using MyProject.Data;
    using MyProject.Global;

    public class StaffAppService : MyProjectAppServiceBase, IStaffAppService
    {
        private readonly IRepository<Staff> staffRepository;
        private readonly IAppFolders appFolders;

        public StaffAppService(IRepository<Staff> staffRepository, IAppFolders appFolders)
        {
            this.staffRepository = staffRepository;
            this.appFolders = appFolders;
        }

        public async Task<PagedResultDto<StaffForView>> GetAllAsync(StaffGetAllInputSto input)
        {
            try
            {
                var filter = this.staffRepository.GetAll()
                         .WhereIf(input != null && !string.IsNullOrEmpty(input.Keyword), e => e.Ma.Contains(input.Keyword) || e.Name.Contains(input.Keyword));
                var totalCount = await filter.CountAsync();
                var query = from o in filter
                            select new StaffForView()
                            {
                                Staff = this.ObjectMapper.Map<StaffSto>(o),
                                //                TrangThai = o.DropdownSingle != null && GlobalModel.TrangThaiHieuLucSorted.ContainsKey((int)o.DropdownSingle) ? GlobalModel.TrangThaiHieuLucSorted[(int)o.DropdownSingle] : string.Empty,
                                //                TrangThaiDuyet = o.AutoCompleteSingle != null && GlobalModel.TrangThaiDuyetSorted.ContainsKey((int)o.AutoCompleteSingle) ? GlobalModel.TrangThaiDuyetSorted[(int)o.AutoCompleteSingle] : string.Empty,
                            };
                var items = query.PageBy(input).ToList();
                return new PagedResultDto<StaffForView>
                {
                    TotalCount = totalCount,
                    Items = items,
                };

            }
            catch (Exception e)
            {

                throw e;
            }
            
        }

        /// <summary>
        /// Kiểm tra thêm mới hay cập nhật.
        /// </summary>
        /// <param name="input">Đầu vào.</param>
        public async Task<int> CreateOrEdit(StaffCreateInput input)
        {
            // check null input
            if (input == null)
            {
                throw new UserFriendlyException(StringResources.NullParameter);
            }

            input.Ma = GlobalFunction.RegexFormat(input.Ma);
            input.Name = GlobalFunction.RegexFormat(input.Name);
            input.Address = GlobalFunction.RegexFormat(input.Address);
            input.Email = GlobalFunction.RegexFormat(input.Email);

            if (this.CheckExist(input.Ma, input.Id))
            {
                return 1;
            }

            // nếu là thêm mới
            if (input.Id == null)
            {
                await this.Create(input);
            }
            else
            {
                // là cập nhật
                await this.Update(input);
            }

            return 0;
        }

        public async Task<StaffCreateInput> GetForEditAsync(EntityDto input)
        {
            if (input == null)
            {
                throw new UserFriendlyException(StringResources.NullParameter);
            }
            //var staff = this.staffRepository.GetAllIncluding(e => e.ListDemoFile).First(e => e.Id == (int)input.Id);
            //var edit = this.ObjectMapper.Map<StaffCreateInput>(staff);
            return await Task.FromResult(new StaffCreateInput());
        }

        public async Task Delete(EntityDto input)
        {
            if (input == null)
            {
                throw new UserFriendlyException(StringResources.NullParameter);
            }
            

            await this.staffRepository.DeleteAsync(input.Id);
        }

        private bool CheckExist(string ma, int? id)
        {
            ma = GlobalFunction.RegexFormat(ma);

            // Nếu query > 0 thì là bị trùng mã => return true
            var query = this.staffRepository.GetAll().Where(e => e.Ma == ma)
                .WhereIf(id != null, e => e.Id != id).Count();
            return query > 0;
        }

        private async Task Create(StaffCreateInput input)
        {
            var create = this.ObjectMapper.Map<Staff>(input);
            await this.staffRepository.InsertAndGetIdAsync(create);
        }

        private async Task Update(StaffCreateInput input)
        {
            var update = await this.staffRepository.GetAsync((int)input.Id);
            this.ObjectMapper.Map(input, update);
        }
    }
}
