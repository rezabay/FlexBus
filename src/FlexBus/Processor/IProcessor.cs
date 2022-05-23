using System.Threading.Tasks;

namespace FlexBus.Processor;

public interface IProcessor
{
    Task ProcessAsync(ProcessingContext context);
}