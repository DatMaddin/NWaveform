using System;
using System.Linq;
using Caliburn.Micro;
using NWaveform.Events;
using NWaveform.NAudio;

namespace NWaveform
{
    public class SamplesHandlerPeakPublisher : IHandle<SamplesReceivedEvent>
    {
        private readonly IEventAggregator _events;
        private readonly IPeakProvider _peakProvider;

        public SamplesHandlerPeakPublisher(IEventAggregator events, IPeakProvider peakProvider)
        {
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _peakProvider = peakProvider ?? throw new ArgumentNullException(nameof(peakProvider));
            _events.Subscribe(this);
        }

        public void Handle(SamplesReceivedEvent samples)
        {
            var start = samples.Start.TotalSeconds;
            var end = start + ((double)samples.Data.Length / samples.WaveFormat.AverageBytesPerSecond);
            var peaks = _peakProvider.Sample(samples.WaveFormat, samples.Data);
            if (peaks.Any())
                _events.PublishOnCurrentThread(new PeaksReceivedEvent(samples.Source, start,end, peaks,samples.CurrentAudioTime));
        }
    }
}
