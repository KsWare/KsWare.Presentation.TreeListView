using System.Collections.ObjectModel;

namespace KsWare.Presentation.TreeListView.TestApp.Model {

	/// <summary>
	/// This class defines a person.
	/// </summary>
	/// <!-- DPE -->
	public class Person {

		#region Fields

		/// <summary>
		/// Stores the id used to identify a person.
		/// </summary>
		private static int s_msId;

		#endregion // Fields.   

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Person"/> class.
		/// </summary>
		public Person() {
			Id = ++s_msId;
			Children = new ObservableCollection<Person>();
		}

		#endregion // Constructor.

		#region Properties

		/// <summary>
		/// Gets or sets the person name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the person id.
		/// </summary>
		public int Id { get; set; }

		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the children list of this person.
		/// </summary>
		public ObservableCollection<Person> Children { get; private set; }

		/// <summary>
		/// Gets or sets the number of item to add when benching the add performance.
		/// </summary>
		public int NumberOfItemsToAdd { get; private set; }

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Returns the current object as string.
		/// </summary>
		/// <returns>The string description of the object.</returns>
		public override string ToString() {
			return Name;
		}

		/// <summary>
		/// Creates a test model.
		/// </summary>
		/// <param name="count1">The number of item to create at the first level.</param>
		/// <param name="count2">The number of item to create at the second level.</param>
		/// <param name="count3">The number of item to create at the third level.</param>
		/// <returns>The model.</returns>
		public static Person CreateTestModel(int count1, int count2, int count3) {
			var model = new Person();
			for
				(var iter1 = 0; iter1 < count1; iter1++) {
				var p1 = new Person() {Name = "Person A" + iter1.ToString()};
				model.Children.Add(p1);
				for
					(var iter2 = 0; iter2 < count2; iter2++) {
					var p2 = new Person() {Name = "Person B" + iter2.ToString()};
					p1.Children.Add(p2);
					for
						(var iter3 = 0; iter3 < count3; iter3++) {
						p2.Children.Add(new Person() {Name = "Person C" + iter3.ToString()});
					}
				}
			}

			return model;
		}

		/// <summary>
		/// Creates the model for the expand or remove test.
		/// </summary>
		public static Person CreateFullTestModel() {
			var root = new Person() {Name = "Root"};

			var person1 = CreateTestModel(100, 0, 0);
			person1.Name = "100 children";
			person1.NumberOfItemsToAdd = 100;
			root.Children.Add(person1);

			var person2 = CreateTestModel(250, 0, 0);
			person2.Name = "250 children";
			person1.NumberOfItemsToAdd = 250;
			root.Children.Add(person2);

			var person3 = CreateTestModel(500, 0, 0);
			person3.Name = "500 children";
			person1.NumberOfItemsToAdd = 500;
			root.Children.Add(person3);

			var person4 = CreateTestModel(1000, 0, 0);
			person4.Name = "1000 children";
			person1.NumberOfItemsToAdd = 1000;
			root.Children.Add(person4);

			var person5 = CreateTestModel(2000, 0, 0);
			person5.Name = "2000 children";
			person1.NumberOfItemsToAdd = 2000;
			root.Children.Add(person5);

			var person6 = CreateTestModel(10000, 0, 0);
			person6.Name = "10000 children";
			person1.NumberOfItemsToAdd = 10000;
			root.Children.Add(person6);

			return root;
		}

		/// <summary>
		/// Creates an empty model.
		/// </summary>
		public static Person CreateEmptyTestModel() {
			var root = new Person() {Name = "Root"};
			root.Children.Add(new Person() {Name = "100 children", NumberOfItemsToAdd = 100});
			root.Children.Add(new Person() {Name = "250 children", NumberOfItemsToAdd = 250});
			root.Children.Add(new Person() {Name = "500 children", NumberOfItemsToAdd = 500});
			root.Children.Add(new Person() {Name = "1000 children", NumberOfItemsToAdd = 1000});
			root.Children.Add(new Person() {Name = "2000 children", NumberOfItemsToAdd = 2000});

			return root;
		}

		#endregion // Methods.

	}

}