using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.Presentation.Tests.Core {

	[TestClass]
	public class GCTests {

		private const long maxGarbage = 1000;

		[TestMethod]
		public void Collect() {
            // Put some objects in memory.
            new MyGCCollectClass().MakeSomeGarbage();
            Console.WriteLine("Memory used before collection:     {0}", GC.GetTotalMemory(false));

            // Collect all generations of memory.
            GC.Collect();
            Console.WriteLine("Memory used after full collection: {0}", GC.GetTotalMemory(true));
        }

		[TestMethod]
		public void GetGeneration() {
			// Create a strong reference to an object.
			MyGCCollectClass myGCCol = new MyGCCollectClass();

			// Put some objects in memory.
			myGCCol.MakeSomeGarbage();

			// Get the generation of managed memory where myGCCol is stored.
			Console.WriteLine("The object is in generation: {0}", GC.GetGeneration(myGCCol));

			// Perform a full garbage collection.
			// Because there is a strong reference to myGCCol, it will
			// not be garbage collected.
			GC.Collect();

			// Get the generation of managed memory where myGCCol is stored.
			Console.WriteLine("The object is in generation: {0}", GC.GetGeneration(myGCCol));

			// Create a WeakReference to myGCCol.
			WeakReference wkref = new WeakReference(myGCCol);
			// Remove the strong reference to myGCCol.
			myGCCol = null;

			// Get the generation of managed memory where wkref is stored.
			Console.WriteLine("The WeakReference to the object is in generation: {0}", GC.GetGeneration(wkref));

			// Perform another full garbage collection.
			// A WeakReference will not survive a garbage collection.
			GC.Collect();

			// Try to get the generation of managed memory where wkref is stored.
			// Because it has been collected, an exception will be thrown.
			try {
				Console.WriteLine("The WeakReference to the object is in generation: {0}", GC.GetGeneration(wkref));
			} catch (Exception e) {
				Console.WriteLine("The WeakReference to the object has been garbage collected: '{0}'", e);
			}
		}

		
		[TestMethod]
        public void CollectX() {
            MyGCCollectClass myGCCol = new MyGCCollectClass();

            // Determine the maximum number of generations the system
	    // garbage collector currently supports.
            Console.WriteLine("The highest generation is {0}", GC.MaxGeneration);

            myGCCol.MakeSomeGarbage();

            // Determine which generation myGCCol object is stored in.
            Console.WriteLine("Generation: {0}", GC.GetGeneration(myGCCol));

            // Determine the best available approximation of the number 
	    // of bytes currently allocated in managed memory.
            Console.WriteLine("Total Memory: {0}", GC.GetTotalMemory(false));

            // Perform a collection of generation 0 only.
            GC.Collect(0);

            // Determine which generation myGCCol object is stored in.
            Console.WriteLine("Generation: {0}", GC.GetGeneration(myGCCol));

            Console.WriteLine("Total Memory: {0}", GC.GetTotalMemory(false));

            // Perform a collection of all generations up to and including 2.
            GC.Collect(2);

            // Determine which generation myGCCol object is stored in.
            Console.WriteLine("Generation: {0}", GC.GetGeneration(myGCCol));
            Console.WriteLine("Total Memory: {0}", GC.GetTotalMemory(false));
        }

		class MyGCCollectClass {
			public void MakeSomeGarbage() {
				Version vt;

				for (int i = 0; i < maxGarbage; i++) {
					// Create objects and release them to fill up memory
					// with unused objects.
					vt = new Version();
				}
			}
			

		}


	}

}
