/**
 * converts the given form to a object. Note, this will provide a partial
 * type with the same keys at type T, however ALL of the forms elements will
 * be added (not just those on type T), so be careful! If the form contains
 * additional form inputs that don't exist on type T, then they won't be
 * available and compile time, but they will be present at runtime
 * @param form the form to convert
 */
export const formToObject = <T extends {}>(form: HTMLFormElement) =>
    Array.from(form.elements).reduce((prev, curr) => {
        const name = curr.getAttribute('name');
        const value = curr.getAttribute('value');
        if (name != null) return { ...prev, [name]: value };
        return prev;
        // the partial type makes all properties optional, so it's safe to assign an
        // empty object to a partial object of T
        // eslint-disable-next-line @typescript-eslint/no-object-literal-type-assertion
    }, {} as Partial<T>);
