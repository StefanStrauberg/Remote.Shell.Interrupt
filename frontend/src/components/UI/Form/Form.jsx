import Wrapper from '../../Wrapper/Wrapper';
import Button from '../Button/Button';
import Input from '../Input/Input';
import Select from '../Select/Select';
import classes from './Form.module.css';

export default function Form({
    types,
    onChange,
    submitText,
    placeholder,
    onSubmit,
}) {
    return (
        <Wrapper>
            <div className={classes.wrapper}>
                {types.map((type) => (
                    <label key={type.name}>
                        <div className={classes.labelText}>{type.name}</div>
                        {type.input === 'input' && (
                            <Input
                                placeholder={placeholder || 'Enter a new info'}
                                onChange={(e) => onChange(e, type.requestName)}
                            />
                        )}
                        {type.input === 'select' && (
                            <Select
                                options={
                                    type.options || [{ option: '', value: '' }]
                                }
                                onChange={(e) => onChange(e, type.requestName)}
                            />
                        )}
                    </label>
                ))}
            </div>
            <div className={classes.submitWrapper}>
                <Button classNames={classes.saveBtn} onClick={onSubmit}>
                    {submitText || 'Save'}
                </Button>
            </div>
        </Wrapper>
    );
}
