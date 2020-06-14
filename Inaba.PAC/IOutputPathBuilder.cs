using System;

namespace Inaba.PAC
{
    public interface IOutputPathBuilder
    {
        String GetOutputPath(String relativePath, Int32 fileIndex);
    }
}