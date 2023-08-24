import { interval, Subscription } from 'rxjs';
import { takeWhile, tap } from 'rxjs/operators';

declare global {
    interface Array<T> {
        remove(elem: T): T;
        removeLast(): T;
        findAndRemove(predicate: (a: T) => boolean): T;
        filterAndRemove(predicate: (a: T) => boolean): T[];
        toggle(elem: T): void;
        distinct(compareFn?: (a: T, b: T) => boolean): Array<T>;
        first(): T;
        last(): T;
        replace(old: T, current: T): T;
        reload(items: T[]): void;
        clear(): void;
        all(predicate: (a: T, i?: number) => boolean): boolean;
        count(predicate: (a: T) => boolean): number;
        atLeast(predicate: (a: T) => boolean, count: number): boolean;
        atMost(predicate: (a: T) => boolean, count: number): boolean;
        sum(predicate: (a: T, i?: number) => number): number;
        average(predicate: (a: T, i?: number) => number): number;
        max<N>(predicate: (a: T) => N, defaultValue?: N): N;
        min<N>(predicate: (a: T) => N, defaultValue?: N): N;
        skip(count: number): Array<T>;
        take(count: number): Array<T>;
        contains(...items: T[]): boolean;
        pushIfNotContained(...items: T[]): void;
        containsAny(...items: T[]): boolean;
        any(): boolean;
        groupBy<N, K = T>(prop: (a: T) => N, selector?: (a: T) => K, keyEquality?: (k1: N, k2: N) => boolean): Array<Group<N, K>>;
        orderBy(...predicate: ((a: T) => any)[]): Array<T>;
        orderByDescending(...predicate: ((a: T) => any)[]): Array<T>;
        orderByReference<N>(reference: N[], equals: (a: T, b: N) => boolean): Array<T>;
        findLast(predicate: (a: T) => boolean): T;
        findLastIndex(predicate: (a: T) => boolean): number;
        as<N>(): Array<N>;
        clone(): Array<T>;
        isEmpty(): boolean;
        except<T>(...items: T[]): Array<T>;
        isFirst<T>(item: T): boolean;
        getPrevious<T>(item: T): T;
        getNext<T>(item: T): T;
        multiply(times: number): T[];
        fillProgressively<T>(items: T[], batchSize: number, timer: number): Subscription;
        excludingLast(): Array<T>;
        isEquivalentTo(other: T[]): boolean;
        equals(other: T[]): boolean;
        batched(size: number): Array<Array<T>>;
        insertAt(item: T, index: number): void;
        insertBefore(newItem: T, existingItem: T): void;
        insertAfter(newItem: T, existingItem: T): void;
        changePosition(previousIndex: number, currentIndex: number): T;
        innerJoin<N, R>(inner: N[], predicate: (o: T, i: N) => boolean, resultSelector: (o: T, i: N) => R): R[];
        leftJoin<N, R>(inner: N[], predicate: (o: T, i: N) => boolean, resultSelector: (o: T, i?: N) => R): R[];
        takeFrom(elem: T): T[];
    }
}

export class Group<TKey, TValue> extends Array<TValue> {

    constructor(
        readonly key: TKey
    ) {
        super();
    }

}

export function arrayExtensions() {

    Array.prototype.remove = function <T>(item: T) {
        const index = this.indexOf(item);

        if (index < 0) {
            return null;
        }

        return this.splice(index, 1)[0];
    };

    Array.prototype.removeLast = function () {
        if (this.isEmpty()) {
            return null;
        }

        return this.splice(this.length - 1, 1)[0];
    };

    Array.prototype.findAndRemove = function <T>(predicate: (a: T) => boolean) {
        const index = this.findIndex(predicate);

        return index < 0 ? null : this.splice(index, 1)[0];
    }


    Array.prototype.filterAndRemove = function <T>(predicate: (a: T) => boolean) {
        return this
            .filter(e => predicate(e))
            .map(e => this.remove(e));
    }

    Array.prototype.toggle = function <T>(item: T) {
        const index = this.indexOf(item);

        if (index < 0) {
            this.push(item);
        } else {
            this.splice(index, 1)[0];
        }
    };

    Array.prototype.distinct = function <T>(compareFn?: (a: T, b: T) => boolean) {
        const result: Array<T> = [];

        if (compareFn) {
            this.forEach(e => result.some(ee => compareFn(e, ee)) || result.push(e));
        } else {
            this.forEach(e => result.indexOf(e) < 0 && result.push(e));
        }

        return result;
    }

    Array.prototype.first = function () {
        return this[0];
    };

    Array.prototype.last = function () {
        return this[this.length - 1];
    };

    Array.prototype.replace = function <T>(old: T, current: T) {
        const index = this.indexOf(old);

        if (index < 0) {
            return;
        }

        this.splice(index, 1, current);
    };

    Array.prototype.clear = function () {
        this.splice(0, this.length);
    };

    Array.prototype.all = function <T>(predicate: (a: T, i: number) => boolean) {
        for (let index = 0; index < this.length; index++) {
            const element = this[index];

            if (!predicate(element, index)) {
                return false;
            }
        }

        return true;
    };

    Array.prototype.sum = function <T>(predicate: (a: T, i?: number) => number) {
        let result = 0;

        this.forEach((e, i) => result += predicate(e, i));

        return result;
    };

    Array.prototype.average = function <T>(predicate: (a: T, i?: number) => number) {
        if (!this.length) {
            return 0;
        }

        const sum = this.sum(predicate);

        return sum / this.length;
    };

    Array.prototype.max = function <T, N>(predicate: (a: T) => N, defaultValue: N = null) {
        let result: N;

        this.forEach(e => result = _max(e, predicate, result));

        return result == null && defaultValue != null
            ? defaultValue
            : result;
    };

    const _max = <T, N>(item: T, predicate: (a: T) => N, max: N) => {
        const value = predicate(item);

        return max == null || value > max
            ? value
            : max;
    };

    Array.prototype.min = function <T, N>(predicate: (a: T) => N, defaultValue: N = null) {
        let result: N;

        this.forEach(e => result = _min(e, predicate, result));

        return result == null && defaultValue != null
            ? defaultValue
            : result;
    };

    const _min = <T, N>(item: T, predicate: (a: T) => N, min: N) => {
        const value = predicate(item);

        return min == null || value < min
            ? value
            : min;
    };

    Array.prototype.skip = function (count: number) {
        const min = count - 1;

        return this.filter((_, i) => i > min);
    };

    Array.prototype.take = function (count: number) {
        return this.filter((_, i) => i < count);
    };

    Array.prototype.contains = function <T>(...items: T[]) {
        return items.all(e => this.indexOf(e) >= 0);
    };

    Array.prototype.containsAny = function <T>(...items: T[]) {
        return items.some(e => this.indexOf(e) >= 0);
    };

    Array.prototype.any = function () {
        return this.length > 0;
    }

    Array.prototype.groupBy = function <T, N, K = T>(prop: (a: T) => N, selector?: (a: T) => K, keyEquality?: (k1: N, k2: N) => boolean) {
        const groups: Array<Group<N, K>> = [];

        for (const element of this) {
            const key = prop(element);
            let group = keyEquality
                ? groups.find(e => keyEquality(e.key, key))
                : groups.find(e => e.key === key);

            if (group == null) {
                group = new Group(key);
                groups.push(group);
            }

            const item = selector
                ? selector(element)
                : element;

            group.push(item);
        }

        return groups;
    };

    Array.prototype.orderBy = function <T>(...predicate: ((a: T) => any)[]) {
        return this.sort((a, b) => _sortIteration(a, b, predicate));
    };

    Array.prototype.orderByDescending = function <T>(...predicate: ((a: T) => any)[]) {
        return this.sort((a, b) => _sortIteration(a, b, predicate) * -1);
    };

    const _sortIteration = (a: any, b: any, predicates: ((x: any) => any)[], index = 0): (1 | 0 | -1) => {
        const predicate = predicates[index];

        if (!predicate) {
            return 0;
        }

        const aValue = predicate(a);
        const bValue = predicate(b);
        const result = aValue < bValue
            ? -1
            : aValue > bValue
                ? 1
                : 0;

        return result == 0
            ? _sortIteration(a, b, predicates, index + 1)
            : result;
    }

    Array.prototype.orderByReference = function <T, N>(reference: N[], equals: (a: T, b: N) => boolean) {
        return (this as T[])
            .orderBy(a =>
                reference
                    .findIndex(b => equals(a, b))
            );
    }

    Array.prototype.findLast = function <T>(predicate: (a: T) => boolean) {
        return this.clone().reverse().find(predicate);
    };

    Array.prototype.findLastIndex = function <T>(predicate: (a: T) => boolean) {
        const item = this.findLast(predicate);

        return this.lastIndexOf(item);
    }

    Array.prototype.as = function <N>() {
        return this as N[];
    };

    Array.prototype.clone = function <T>() {
        return this.concat([]) as T[];
    };

    Array.prototype.isEmpty = function () {
        return this.length === 0;
    };

    Array.prototype.pushIfNotContained = function <T>(...items: T[]) {
        for (const item of items) {
            if (!this.contains(item)) {
                this.push(item);
            }
        }
    };

    Array.prototype.reload = function <T>(items: T[]) {
        (this as T[]).splice(0, this.length, ...items);
    };

    Array.prototype.except = function <T>(...items: T[]) {
        return this.filter(e => !items.contains(e));
    };

    Array.prototype.isFirst = function <T>(item: T) {
        return this.indexOf(item) === 0;
    }

    Array.prototype.getPrevious = function <T>(item: T) {
        const index = this.indexOf(item);

        return this[index - 1];
    }

    Array.prototype.getNext = function <T>(item: T) {
        const index = this.indexOf(item);

        return this[index + 1];
    }

    Array.prototype.multiply = function <T>(times: number) {
        const result: T[] = [];

        for (let index = 0; index < times; index++) {
            result.push(...this);
        }

        return result;
    }

    Array.prototype.fillProgressively = function <T>(items: T[], batchSize: number, timer: number) {
        return interval(timer)
            .pipe(
                takeWhile(() => this.length !== items.length),
                tap(e => this.push(...items.skip(e * batchSize).take(batchSize)))
            )
            .subscribe();
    }

    Array.prototype.excludingLast = function <T>() {
        this.splice(this.length - 1, 1);

        return this as T[];
    }

    Array.prototype.count = function <T>(predicate: (a: T) => boolean): number {
        return this
            .filter(e => predicate(e))
            .length;
    }

    Array.prototype.atLeast = function <T>(predicate: (a: T) => boolean, count: number) {
        if (count > this.length) {
            return false;
        }

        let i = 0;

        for (const item of this) {
            if (predicate(item)) {
                i++;

                if (i === count) {
                    return true;
                }
            }
        }

        return false;
    }

    Array.prototype.atMost = function <T>(predicate: (a: T) => boolean, count: number) {
        let i = 0;

        for (const item of this) {
            if (predicate(item)) {
                i++;

                if (i > count) {
                    return false;
                }
            }
        }

        return true;
    }

    Array.prototype.isEquivalentTo = function <T>(other: T[]) {
        return other != null &&
            other.length === this.length &&
            this.contains(...other);
    }

    Array.prototype.equals = function <T>(other: T[]) {
        if (other == null) {
            return false;
        }

        if (other === this) {
            return true;
        }

        if (other.length !== this.length) {
            return false;
        }

        for (let i = 0; i < this.length; i++) {
            const a = this[i] as any;
            const b = other[i] as any;

            if (a == null && b == null) {
                continue;
            }
            if (
                (a != null && b == null) ||
                (a == null && b != null)
            ) {
                return false;
            }

            if (typeof (a.equals) === 'function' && typeof (b.equals) === 'function') {
                if (a.equals(b)) {
                    continue;
                }

                return false;
            }

            if (typeof (a.valueOf) === 'function' && typeof (b.valueOf) === 'function') {
                if (a.valueOf() === b.valueOf()) {
                    continue;
                }

                return false;
            }

            if (a !== b) {
                return false;
            }
        }

        return true;
    }

    Array.prototype.batched = function <T>(size: number) {
        let current: T[];
        const result: T[][] = [];

        for (let i = 0; i < this.length; i++) {
            const module = i % size;
            const item = this[i];

            if (!module) {
                current = [];
                result.push(current);
            }

            current.push(item);
        }

        return result;
    }

    Array.prototype.insertAt = function <T>(item: T, index: number): void {
        this.splice(index, 0, item);
    }

    Array.prototype.insertBefore = function <T>(newItem: T, existingItem: T): void {
        const index = this.indexOf(existingItem);

        this.splice(index, 0, newItem);
    }

    Array.prototype.insertAfter = function <T>(newItem: T, existingItem: T): void {
        const index = this.indexOf(existingItem) + 1;

        this.splice(index, 0, newItem);
    }

    Array.prototype.changePosition = function <T>(previousIndex: number, currentIndex: number): T {
        const item = this[previousIndex];

        if (currentIndex === previousIndex) {
            return item;
        }

        if (previousIndex < currentIndex) {
            this.splice(previousIndex, 1);
            this.splice(currentIndex, 0, item);
        } else {
            this.splice(currentIndex, 0, item);
            this.splice(previousIndex + 1, 1);
        }

        return item;
    }

    Array.prototype.innerJoin = function <T, N, R>(inner: N[], predicate: (o: T, i: N) => boolean, resultSelector: (o: T, i: N) => R) {
        const result = new Array<R>();

        for (const outerItem of (this as T[])) {
            for (const innerItem of inner) {
                if (predicate(outerItem, innerItem)) {
                    const resultItem = resultSelector(outerItem, innerItem);
                    result.push(resultItem);
                }
            }
        }

        return result;
    }

    Array.prototype.leftJoin = function <T, N, R>(inner: N[], predicate: (o: T, i: N) => boolean, resultSelector: (o: T, i?: N) => R) {
        const result = new Array<R>();

        for (const outerItem of (this as T[])) {
            let found = false;

            for (const innerItem of inner) {
                if (!predicate(outerItem, innerItem)) {
                    continue;
                }

                found = true;

                const resultItem = resultSelector(outerItem, innerItem);
                result.push(resultItem);
            }

            if (!found) {
                const resultItem = resultSelector(outerItem, null);
                result.push(resultItem);
            }
        }

        return result;
    }

    Array.prototype.takeFrom = function <T>(elem: T) {
        const index = this.indexOf(elem);

        if (index < 0) {
            return [];
        }

        return this
            .filter((_, i) => i >= index);
    }

}