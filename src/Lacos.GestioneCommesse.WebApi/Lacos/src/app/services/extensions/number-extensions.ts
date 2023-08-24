declare global {
    interface Number {
        subtract(other: number): number;
        add(other: number): number;
        multiply(other: number): number;
        divide(other: number): number;
        round(): number;
        percent(percentage: number): number;
        percentRemaining(percentage: number): number;
    }
}

export function numberExtensions() {

    Number.prototype.subtract = function (other: number) {
        const value = this as number;

        return (value - other).round();
    }

    Number.prototype.add = function (other: number) {
        const value = this as number;

        return (value + other).round();
    }

    Number.prototype.multiply = function (other: number) {
        const value = this as number;

        return (value * other).round();
    }

    Number.prototype.divide = function (other: number) {
        const value = this as number;

        return (value / other).round();
    }

    Number.prototype.round = function () {
        const value = this as number;

        return Math.round(value * 100) / 100;
    }

    Number.prototype.percent = function (percentage: number) {
        const value = this as number;

        return value.multiply(percentage).divide(100);
    }

    Number.prototype.percentRemaining = function (percentage: number) {
        const value = this as number;

        return value.subtract(value.percent(percentage));
    }

}