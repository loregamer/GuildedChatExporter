using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;

namespace GuildedChatExporter.Gui.Utils.Extensions;

public static class Extensions
{
    public static IDisposable WatchProperty<T, TProperty>(
        this T source,
        Expression<Func<T, TProperty>> propertyExpression,
        Action handler
    )
        where T : INotifyPropertyChanged
    {
        var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;

        return Observable
            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h
            )
            .Where(args => args.EventArgs.PropertyName == propertyName)
            .Subscribe(_ => handler());
    }

    public static IDisposable WatchProperty<T>(
        this ObservableCollection<T> source,
        Expression<Func<ObservableCollection<T>, int>> propertyExpression,
        Action handler
    )
    {
        var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;

        var disposable = new CompositeDisposable();

        // Watch for collection changes
        disposable.Add(
            Observable
                .FromEventPattern<
                    NotifyCollectionChangedEventHandler,
                    NotifyCollectionChangedEventArgs
                >(h => source.CollectionChanged += h, h => source.CollectionChanged -= h)
                .Subscribe(_ => handler())
        );

        // Watch for property changes
        disposable.Add(
            Observable
                .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => ((INotifyPropertyChanged)source).PropertyChanged += h,
                    h => ((INotifyPropertyChanged)source).PropertyChanged -= h
                )
                .Where(args => args.EventArgs.PropertyName == propertyName)
                .Subscribe(_ => handler())
        );

        return disposable;
    }

    public static MaterialThemeBase LocateMaterialTheme<T>(this Application app)
        where T : MaterialThemeBase =>
        app.Styles.OfType<T>().FirstOrDefault()
        ?? throw new InvalidOperationException("Material theme not found");
}
