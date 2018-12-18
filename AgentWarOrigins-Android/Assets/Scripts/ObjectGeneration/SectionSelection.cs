using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EndlessRunner
{
    /**
     * ProbabilityRandom - If the value is less than the probability, a new section will randomly be chosen between startSection and endSection
     * ProbabilityLoop - If the value is less than the probability, a new section will be chosen in a loop - 0, 1, 2, 0, 1, etc
     * Linear - The section is determined by the distance, no randomness involved
     */
    public enum SectionSelectionType { ProbabilityRandom, ProbabilityLoop, Linear, None }

    /**
     * Sections define groups of objects that can spawn at certain times. For example you may want a set of objects to spawn when indoors and a different
     * set to spawn when outdoors
     */
    public class SectionSelection : MonoBehaviour
    {
        static public SectionSelection instance;

        // the way to select a new section
        public SectionSelectionType sectionSelectionType;

        // If true, you must provide transitions from one section to another for every combination of sections for both platforms and scenes
        public bool useSectionTransitions;

        // the start section, used by the probability type
        [HideInInspector]
        public int startSection;

        // the end section (inclusive), used by the probability type
        [HideInInspector]
        public int endSection;

        // the list of probabilities or sections, depending on the type
        [HideInInspector]
        public DistanceValueList sectionList;

        private DistanceValueList sceneSectionList;
        private int activeSceneSection;

        private InfiniteObjectHistory infiniteObjectHistory;

        public void Awake()
        {
            instance = this;
        }

        public void Start()
        {
            infiniteObjectHistory = InfiniteObjectHistory.instance;

            // create a new section list for the platforms and scenes since their distances are different
            if (sectionSelectionType == SectionSelectionType.Linear) {  
                sceneSectionList = new DistanceValueList(sectionList.loop, sectionList.loopBackToIndex);
                sceneSectionList.values = sectionList.values;
                sceneSectionList.Init();
            } else {
                sectionList.Init();
            }
        }

        // returns the section based off of the distance.
        public int GetSection(float distance)
        {
            if (sectionSelectionType == SectionSelectionType.None) {
                return 0;
            }

            int activeSection = (activeSceneSection);
            if (sectionSelectionType == SectionSelectionType.ProbabilityLoop || sectionSelectionType == SectionSelectionType.ProbabilityRandom) {
         
                    if (Random.value < sectionList.GetValue(distance)) {
                        if (sectionSelectionType == SectionSelectionType.ProbabilityRandom) {
                            activeSection = Random.Range(startSection, endSection + 1);
                        } else {
                            if (startSection > endSection) {
                                activeSection = (activeSection + 1) % (startSection - endSection + 1); // inclusive
                            } else {
                                activeSection = startSection;
                            }
                        }
                }
            } else { // linear
                    activeSection = (int)sceneSectionList.GetValue(distance);
                }
                activeSceneSection = activeSection; 

            return activeSection;
        }

        public void ResetValues()
        {
             activeSceneSection = 0;

            if (sectionSelectionType == SectionSelectionType.Linear) {
                sceneSectionList.ResetValues();
            } else {
                sectionList.ResetValues();
            }
        }
    }
}