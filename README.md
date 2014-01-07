StressTest
==========

A stress test tool for uploading/downloading data to/from a data platform in limited rate and test for the highest rate

1. Action.cs is the base class for something need to do. LogAppendAction.cs, UploadAction.cs and DownloadAction.cs are classes derived form Action.cs.

2. RateProfile.cs is used to update the current and average rate.

3. TokenController.cs is used to manage the allowed max rate.

4. ProbeRate.cs is used to automatic test for different combines of datasize and threads.
