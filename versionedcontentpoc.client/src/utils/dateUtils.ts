import { differenceInCalendarDays, format, isValid, parseISO } from 'date-fns';

export function relativeDate(input: Date | string | undefined | null): string {
    if (!input)
        return "-";

    const date = typeof input === 'string'
        ? parseISO(input)
        : input;

    if (!isValid(date))
        return "-";

    const diffInDays = differenceInCalendarDays(new Date(), date);

    if (diffInDays === 0) {
        return "Today";
    }

    if (diffInDays === 1)
        return "Yesterday";

    if (diffInDays >= 2 && diffInDays <= 6)
        return `${diffInDays} days ago`;

    return format(date, 'yyyy-MM-dd');
}

export function relativeDateTime(input: Date | string | undefined | null): string {
    if (!input)
        return "-";

    const date = typeof input === 'string'
        ? parseISO(input)
        : input;

    if (!isValid(date))
        return "-";

    const diffInDays = differenceInCalendarDays(new Date(), date);

    const time = format(date, 'HH:mm');

    if (diffInDays === 0)
        return `Today at ${time}`;
    
    if (diffInDays === 1)
        return `Yesterday at ${time}`;

    return format(date, 'yyyy-MM-dd HH:mm');
}

export const formatDateString = (dateString: string | null | undefined, dateFormat: string): string => {
    if (!dateString)
        return "-";

    return format(new Date(dateString), dateFormat);
}