import Wrapper from '../../Wrapper/Wrapper';
import Button from '../Button/Button';
import Input from '../Input/Input';
import Select from '../Select/Select';
import classes from './Form.module.css';

export default function Form({ types, onChange, submitText, onSubmit }) {
    return (
        <Wrapper>
            <div className={classes.wrapper}>
                {types.map((type) => (
                    <label key={type.name}>
                        <div className={classes.labelText}>{type.name}</div>
                        {type.input === 'input' && (
                            <Input
                                defaultValue={type.defaultValue}
                                placeholder={'Enter a new info...'}
                                onChange={(e) =>
                                    onChange
                                        ? onChange(e, type.onChangeName)
                                        : null
                                }
                            />
                        )}
                        {type.input === 'select' && (
                            <Select
                                options={
                                    type.options || [{ option: '', value: '' }]
                                }
                                onChange={(e) =>
                                    onChange
                                        ? onChange(e, type.requestName)
                                        : null
                                }
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
