import { DateTime, DurationLike } from 'luxon';

declare global {
    interface Date {
        toDateWithoutTime?(): Date;
        toOffsetString?(): string;
        addYears?(years: number): Date;
        addMonths?(months: number): Date;
        addDays?(days: number): Date;
        addMinutes?(minutes: number): Date;
        addHours?(hours: number): Date;
        getDifferenceInMinutes?(date: Date): number;
        add?(duration: DurationLike): Date;
        subtract?(duration: DurationLike): Date;
        isAfter?(date: Date): boolean;
        isAfterOrEquals?(date: Date): boolean;
        isBefore?(date: Date): boolean;
        isBeforeOrEquals?(date: Date): boolean;
        isToday?(): boolean;
        isFuture?(): boolean;
        isPast?(): boolean;
        hasSameDate?(other: Date): boolean;
        hasSameDateAndTime?(other: Date): boolean;
        hasSameMonth?(other: Date): boolean;
        getFirstDateOfMonth?(): Date;
        getAge?(): number;
        isUnderage?(): boolean;
        isAdult?(): boolean;
        clone?(): Date;
    }
}

export function dateExtensions() {

    Date.prototype.toDateWithoutTime = function () {
        return new Date(this.getFullYear(), this.getMonth(), this.getDate());
    }

    Date.prototype.toOffsetString = function () {
        return DateTime.fromJSDate(this).toISO();
    }

    Date.prototype.addYears = function (years: number) {
        return this.add({ years });
    }

    Date.prototype.addMonths = function (months: number) {
        return this.add({ months });
    }

    Date.prototype.addDays = function (days: number) {
        return this.add({ days });
    }

    Date.prototype.addMinutes = function (minutes: number) {
        return this.add({ minutes });
    }

    Date.prototype.addHours = function (hours: number) {
        return this.add({ hours });
    }

    Date.prototype.getDifferenceInMinutes = function (date: Date) {
        return (this.getTime() - date.getTime()) / 1000 / 60;
    }

    Date.prototype.add = function (duration: DurationLike) {
        return DateTime.fromJSDate(this).plus(duration).toJSDate();
    }

    Date.prototype.subtract = function (duration: DurationLike) {
        return DateTime.fromJSDate(this).minus(duration).toJSDate();
    }

    Date.prototype.isToday = function () {
        return this.hasSameDate(new Date());
    }

    Date.prototype.isAfter = function (date: Date) {
        return this.getTime() > date.getTime();
    }

    Date.prototype.isAfterOrEquals = function (date: Date) {
        return this.getTime() >= date.getTime();
    }

    Date.prototype.isBefore = function (date: Date) {
        return this.getTime() < date.getTime();
    }

    Date.prototype.isBeforeOrEquals = function (date: Date) {
        return this.getTime() <= date.getTime();
    }

    Date.prototype.isFuture = function () {
        return this.isAfter(new Date());
    }

    Date.prototype.isPast = function () {
        return this.isBefore(new Date());
    }

    Date.prototype.hasSameDate = function (other: Date) {
        return other != null &&
            this.getFullYear() === other.getFullYear() &&
            this.getMonth() === other.getMonth() &&
            this.getDate() === other.getDate();
    }

    Date.prototype.hasSameDateAndTime = function (other: Date) {
        return other != null &&
            this.getTime() === other.getTime();
    }

    Date.prototype.hasSameMonth = function (other: Date) {
        return other != null &&
            this.getMonth() === other.getMonth() &&
            this.getFullYear() === other.getFullYear();
    }

    Date.prototype.getFirstDateOfMonth = function () {
        return new Date(this.getFullYear(), this.getMonth(), 1);
    }

    Date.prototype.getAge = function () {
        const dateTime = DateTime.fromJSDate(this);
        const years = -dateTime.diffNow('years').years;
        const age = Math.floor(years);

        return age;
    }

    Date.prototype.isUnderage = function () {
        return this.getAge() < 15;
    }

    Date.prototype.isAdult = function () {
        return this.getAge() >= 18;
    }

    Date.prototype.clone = function () {
        return new Date(this);
    }

    // Date.prototype.toJSON = function () {
    //     return this.toOffsetString();
    // }

}
