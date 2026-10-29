using System;
using System.Collections.Generic;

public class SectionSequenceBuilder
{
    private readonly TrackSection[] _sections;

    public SectionSequenceBuilder(TrackSection[] sections)
    {
        if (sections == null || sections.Length == 0)
        {
            throw new ArgumentException("Track sections are not assigned.");
        }

        _sections = (TrackSection[])sections.Clone();
    }

    public List<TrackSection> Build(int level, int seed, int challengeBlockCount = 1)
    {
        if (level < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(level));
        }

        if (challengeBlockCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(challengeBlockCount));
        }

        var random = new Random(seed);
        var sequence = new List<TrackSection>
        {
            SelectSection(TrackSectionType.Start, level, random),
            SelectSection(TrackSectionType.Safe, level, random)
        };

        for (int i = 0; i < challengeBlockCount; i++)
        {
            sequence.Add(SelectSection(TrackSectionType.Bonus, level, random));
            sequence.Add(SelectSection(TrackSectionType.Challenge, level, random));
            sequence.Add(SelectSection(TrackSectionType.Safe, level, random));
        }

        sequence.Add(SelectSection(TrackSectionType.Finish, level, random));

        return sequence;
    }

    private TrackSection SelectSection(TrackSectionType type, int level, Random random)
    {
        int totalWeight = 0;

        foreach (TrackSection section in _sections)
        {
            if (IsEligible(section, type, level))
            {
                totalWeight += section.SelectionWeight;
            }
        }

        if (totalWeight <= 0)
        {
            throw new InvalidOperationException($"No available TrackSection of type {type} for level {level}.");
        }

        int roll = random.Next(totalWeight);

        foreach (TrackSection section in _sections)
        {
            if (IsEligible(section, type, level) == false)
            {
                continue;
            }

            roll -= section.SelectionWeight;

            if (roll < 0)
            {
                return section;
            }
        }

        throw new InvalidOperationException($"Failed to select TrackSection of type {type}.");
    }

    private static bool IsEligible(TrackSection section, TrackSectionType type, int level)
    {
        return section != null && section.Type == type && section.Prefab != null && section.IsAvailableForLevel(level) && section.SelectionWeight > 0;
    }
}