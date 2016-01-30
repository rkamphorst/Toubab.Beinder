# Beinder: simple data binding

From [Wikipedia](https://en.wikipedia.org/wiki/Data_binding):

> Data binding is the process that establishes a connection between the application UI (User Interface) and Business
> logic. If the settings and notifications are correctly set, the data reflects changes when made. It can also mean that
> when the UI is changed, the underlying data will reflect that change.

There are a lot of implementations of data binding, in C# and beyond. So why this library? I've written up a small 
[justification](#justification) for myself below. It explains the main goals for Beinder:

1. **Only do binding.**    
2. **Be loosely coupled to the software that uses it.**    
3. **Be adaptable through extension.**    
4. **Be usable with minimal configuration.**    

## TL;DR

	/* Create binder */
	var binder = new Binder();

	/* Establish bindings.
	 * Store the bindings in this._bindings to make sure it is not garbage collected.
	 * This is typically done when the view appears (*not* when it is created!)
	 */
	this._bindings = binder.Bind(viewModel, view);

	/* Destroy bindings.
	 * Typically when the view disappears.
	 */
	this._bindings.Dispose();
	this._bindings = null;

## Features

* [Pattern agnostic](#pattern-agnostic)
* [Name-based binding](#name-based-binding)
* [Recursive binding](#recursive-binding)
* [Binding across object boundaries](#binding-across-object-boundaries)
* [Binding of collections](#binding-of-collections) (not yet implemented)
* [Value conversion](#value-conversion) (not yet implemented)
* [Value change propagation](#value-change-propagation)
* [Dynamic rebinding](#dynamic-rebinding)
* [Adaptation through extension](#adaptation-through-extension)

### Pattern agnostic

Although designed with the MVVM pattern in mind, the library does not know of views, 
view models or business logic. It only knows of objects that have properties, events and
event handlers that need to be bound in some way.

This results in the view and view model sides being treated exactly the same. The binder 
only knows they are objects, not what function they have.

And although in MVVM, you typically only bind two objects, it is possible to use the binder
to bind more than two. If, for some reason, you need to do this, just pass more objects to
`Binder.Bind()`. 

### Name-based binding

When you bind two (or more) objects, properties with the same name get bound to each other. 

* **The only way to bind two properties to each other is to give them the same name.**    
  There is no other (explicit) way to indicate that one property should be bound to 
  a specific property on another object.
  
* **Bindings are two-way when possible.**    
  If two properties are bound to each other and one of them is read-only, obviously values
  can only propagate from the read-only property to the write-enabled property. However,
  when all bound properties are read-write, values can and will propagate from any of them
  to any of them.
  
* **No per-property data conversions can be specified**.     
  If a property could not be set because (for example) it has the wrong data type, this 
  will fail silently. However, if there are 'properties of properties' (a.k.a. *child properties*) 
  that match, those will be bound to each other instead 
  (see [Recursive binding](#recursive-binding)).

Example:

```csharp
/* ViewModelClass and ViewClass both have a string property MyProperty,
 * and they both implement INotifyPropertyChanged
 */
var view = new ViewClass { MyProperty = "aaa" };
var viewModel = new ViewModelClass { MyProperty = "bbb" };

this._bindings = binder.Bind(viewModel, view);

//  viewModel.MyProperty is now bound to view.MyProperty.
//  view.MyProperty will have the value "bbb".

view.MyProperty = "ccc";      // propagates "ccc" to viewModel.MyProperty
viewModel.MyProperty = "ddd"; // propagates "ddd" to view.MyProperty
```

### Recursive binding

If values of properties that are bound have matching properties, those *child properties* 
will be bound as well. This can be extended to child properties of child properties, and 
so on.

Example:

```csharp
/* ViewModelClass, ViewClass, ControlModelClass and ControlClass all 
 * implement interface INotifyPropertyChanged.
 */
var view = new ViewClass() { MyControl = new ControlClass { Label = "aaa" } };
var viewModel = new ViewModelClass() { MyControl = new ControlModelClass { Label = "bbb" } };

this._bindings = binder.Bind(viewModel, view);

//  viewModel.MyControl.Label is now bound to view.MyControl.Label.
//  both will have the value "bbb".

view.MyControl.Label = "ccc";      // propagates "ccc" to viewModel.MyControl.Label
viewModel.MyControl.Label = "ddd"; // propagates "ddd" to view.MyControl.Label
```

### Binding across object boundaries

If a property's name is a concatenation of another property's name and the name of one of 
it's child properties, the former property is bound to the child property. Again, this
can be extended to child properties of child properties, etc.

Example:

```csharp
/* ViewModelClass, ViewClass, and ControlClass all implement interface 
 * INotifyPropertyChanged.
 */
var view = new ViewClass() { MyControl = new ControlClass { Label = "aaa" } };
var viewModel = new ViewModelClass() { MyControlLabel = "bbb" };

this._bindings = binder.Bind(viewModel, view);

//  viewModel.MyControlLabel is now bound to view.MyControl.Label.
//  both will have the value "bbb".

view.MyControl.Label = "ccc";      // propagates "ccc" to viewModel.MyControlLabel
viewModel.MyControlLabel = "ddd";  // propagates "ddd" to view.MyControl.Label
```

### Binding of collections

**TODO: not yet implemented**

### Value conversions

**TODO: not yet implemented**

### Value change propagation

When `Binder.Bind()` is called, properties are bound, and then values are propagated. 
The values are propagated from properties on the first parameter to properties
on the other parameters. Recall that in in the previous examples, the bound
properties would initially be set to the value of the `viewModel`'s property: this
is because `viewModel` was always the first parameter to `Bind()`.

After a property has been bound, and its value changes, its value can be propagated to the
other properties it is bound to. However, for this to happen, an event is needed that is
raised whenever the property changes. This needs to be implemented in the class/object the
property comes from. There are two standard ways in order to do this:

1. Implement `INotifyPropertyChanged` on the objects you want to bind.    
   See also
   [NotifyPropertyChangedScanner](Toubab.Beinder/PropertyScanners/NotifyPropertyChangedScanner.cs).
2. Implement an event (handler type `EventHandler`) for each property you wan to bind, 
   with the same name, but postfixed with "Changed". Raise it whenever the property is 
   set with a new value.    
   See also
   [ReflectionScanner](Toubab.Beinder/PropertyScanners/ReflectionScanner.cs).

### Dynamic rebinding

Whenever a property's value changes, and this new value is propagated to other properties,
what happens to the property's *child properties*? They get rebound.

Consider the following example.

```csharp
/* ViewModelClass, ViewClass, ControlModelClass, ControlClass and TextControlClass all 
 * implement interface INotifyPropertyChanged.
 */
var view = new ViewClass() { MyControl = new ControlClass { Label = "aaa" } };
var viewModel = new ViewModelClass() { MyControl = new ControlModelClass { Label = "bbb", Text = "ccc" };

this._bindings = binder.Bind(viewModel, view);

//  viewModel.MyControl.Label is now bound to view.MyControl.Label.
//  Both will have the value "bbb".
//  viewModel.MyControl.Text is not bound to anything and will have value null.

view.MyControl = new TextControlClass { Text = "ddd" };

// viewModel.MyControl.Text gets *rebound* to view.MyControl.Text.
// Both will now have value "ddd".
// viewModel.MyControl.Label will still have value "bbb".
```

### Adaptation through extension

#### Properties: `ICustomProperty<T>`

#### Mixins: `IMixin<T>`

#### Namespace preference: affinity and specialization

#### Scanners: `IScanner`

#### Path Parsers: `IPathParser`

## Justification

So why roll my own binding library? 

(Data) binding is implememented as part of many frameworks, for many languages and for many programming environments (see
the mentioned wikipedia page for more examples). Examples for C# include a myriad of UI frameworks that Microsoft has
made over the years (Windows Forms, WPF, ASP.NET, ...), and the last couple of years the various MVVM frameworks that
have sprung up to support cross-platform app development in C# (examples are
[MvvmCross](https://github.com/MvvmCross/MvvmCross), [MVVM Light Toolkit](https://mvvmlight.codeplex.com/), 
[Xamarin Forms](https://xamarin.com/forms)).

Most existing implementations, IMHO, have the following drawbacks:

1. They are *part of* a larger framework, and cannot easily be used outside that framework.
   Especially MvvmCross has a splendid binding implementation; however, choosing its binding framework
   without pulling in the rest into your project is difficult (I did succeed, though).
2. They are complex and hard to configure. This is mostly solved by having great IDE support and 
   great documentation. However, if you do not possess the right IDE and/or have a specific problem that
   is not covered by IDE or docs, or there is a small bug that needs to be solved, you need to dive
   into the framework's code and learn all of its intricate details. 
3. They give too much freedom in the wrong places. This results in complexity where it is not needed,
   code that is difficult to read, and View-ViewModel bindings that are hard to track.
   
Therefore, the goals of Beinder are to:

1. **Only do binding.**    
   ...but do it as well as any other framework, or better. 
2. **Be loosely coupled to the software that uses it.**    
   The code/configuration footprint related to the use of this library should be as small as possible and in a small number of places.
3. **Be adaptable through extension.**    
   Any aspect of the library that could need adaptation, e.g. to a specific platform or toolkit, should be adaptable through extension.
4. **Be usable with minimal configuration.**    
   Have a sensible default configuration that handles almost all of the cases, and use *adaptation through extension* for the rest.
   
However, the most important reason to make this library is... well because it's fun :-)