using System.Collections.Generic;
using Web_CuaHangCafe.Models;

public interface ITinTucRepository
{
    IEnumerable<TbTinTuc> GetAll();
    TbTinTuc GetById(int id);
    void Create(TbTinTuc tinTuc);
    void Update(TbTinTuc tinTuc);
    void Delete(int id);
}
